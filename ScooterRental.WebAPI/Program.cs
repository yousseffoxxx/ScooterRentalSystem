namespace ScooterRental.WebAPI
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "appsettings.Shared.json");

            builder.Configuration.AddJsonFile(sharedConfigPath, optional: true, reloadOnChange: true);

            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration) // Reads log levels from appsettings
                    .Enrich.FromLogContext() // Adds extra details to every log
                    .WriteTo.Console() // Prints to the Visual Studio terminal
                    .WriteTo.File("Logs/scooter-api-log-.txt", rollingInterval: RollingInterval.Day) // Creates a new text file every day!
                    .WriteTo.Seq("http://host.docker.internal:5341");
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

            #region Add services to the container

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();         
            
            builder.Services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => 
                {
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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.Configure<PaymobOptions>(builder.Configuration.GetSection("PaymobSettings"));

            builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("MqttSettings"));

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

            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

            builder.Services.AddHostedService<RedisZoneSubscriberWorker>();

            builder.Services.AddHttpClient<IAiVerificationService, AiVerificationService>(client =>
            {
                var aiUrl = builder.Configuration.GetSection("Urls")["AiService"];
                client.BaseAddress = new Uri(aiUrl);
            });

            builder.Services.AddSignalR();

            // 1. Register Data Access (Database, Redis, Repositories)
            builder.Services.AddPersistenceInfrastructure(builder.Configuration);

            // 2. Register Business Logic (Services, Firebase, AI)
            builder.Services.AddApplicationServices(builder.Configuration);

            // 3. Register Presentation (SignalR Implementations, Controllers, etc.)
            builder.Services.AddPresentation();

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

            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseCors("CORSPolicy");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<AdminHub>("/hubs/admin");
            app.MapHub<RiderHub>("/hubs/rider");

            #endregion

            app.Run();
        }
    }
}