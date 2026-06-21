namespace ScooterRental.WebAPI
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container

            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration) // Reads log levels from appsettings
                    .Enrich.FromLogContext() // Adds extra details to every log
                    .WriteTo.Console() // Prints to the Visual Studio terminal
                    .WriteTo.File("Logs/scooter-api-log-.txt", rollingInterval: RollingInterval.Day) // Creates a new text file every day!
                    .WriteTo.Seq("http://localhost:5341");
            });

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new Microsoft.OpenApi.Models.OpenApiComponents();
                    document.Components.SecuritySchemes = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiSecurityScheme>
                    {
                        ["Bearer"] = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                            Scheme = "bearer",
                            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                            BearerFormat = "JWT"
                        }
                    };

                    // This loops through all your controllers and adds the Padlock icon to them!
                    foreach (var path in document.Paths.Values)
                    {
                        foreach (var operation in path.Operations.Values)
                        {
                            operation.Security.Add(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                            {
                                [new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                                {
                                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                    {
                                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                }] = Array.Empty<string>()
                            });
                        }
                    }
                    return Task.CompletedTask;
                });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    options => options.UseNetTopologySuite());
            });

            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.Configure<PaymobOptions>(builder.Configuration.GetSection("PaymobSettings"));

            builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("MqttSettings"));

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();         
            builder.Services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidAudience = jwtOptions.Audience,
                    ValidIssuer = jwtOptions.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),

                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin();
                    policyBuilder.AllowAnyHeader();
                    policyBuilder.AllowAnyMethod();
                });
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>((_) =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnectionString"));
            });

            builder.Services.AddHostedService<RedisZoneSubscriberWorker>();

            builder.Services.AddSingleton<IZoneCacheService, ZoneCacheService>();

            builder.Services.AddHttpClient<IAiVerificationService, AiVerificationService>(client =>
            {
                var aiUrl = builder.Configuration.GetSection("Urls")["AiService"];
                client.BaseAddress = new Uri(aiUrl);
            });

            builder.Services.AddScoped<IActiveRideCacheRepository, ActiveRideCacheRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IScooterTelemetryRepository, ScooterTelemetryRepository>();
            builder.Services.AddScoped<IRedisZoneEventPublisher, RedisZoneEventPublisher>();
            builder.Services.AddScoped<IMqttCommandService, MqttCommandService>();
            builder.Services.AddScoped<IDataSeeder, DataSeeder>();
            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IScooterSecretCacheRepository, ScooterSecretCacheRepository>();
            
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            #endregion

            var app = builder.Build();

            #region DataSeeding / Load Zones
            using var scope = app.Services.CreateScope();
            {
                var objectOfDataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

                var zoneCache = scope.ServiceProvider.GetRequiredService<IZoneCacheService>();

                await zoneCache.ReloadCacheAsync();

                await objectOfDataSeeding.DataSeedAsync();
            }
            #endregion

            #region Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            var firebasePath = app.Configuration.GetRequiredSection("Firebase")["CredentialPath"];

            if (FirebaseApp.DefaultInstance == null)
            {
                var credential = CredentialFactory.FromFile<ServiceAccountCredential>(firebasePath).ToGoogleCredential();

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            //app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseCors("CORSPolicy");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}