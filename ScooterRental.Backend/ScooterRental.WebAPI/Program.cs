using ScooterRental.Domain.Models.Auth;

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
                    .WriteTo.File("Logs/scooter-api-log-.txt", rollingInterval: RollingInterval.Day); // Creates a new text file every day!
            });

            // Tells .NET 9 OpenAPI to add the JWT Authorization requirement to your endpoints
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

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

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

            builder.Services.AddSingleton<IConnectionMultiplexer>((_) =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnectionString"));
            });

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<IScooterTelemetryRepository, ScooterTelemetryRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRedisZoneEventPublisher, RedisZoneEventPublisher>();
            builder.Services.AddSingleton<IZoneCacheService, ZoneCacheService>();
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            #endregion
            
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                // We get the RoleManager directly from the dependency injection container
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                // Define the roles your app needs
                string[] roles = { "Customer", "Admin", "Support" };

                foreach (var role in roles)
                {
                    // If the role doesn't exist in the database, create it!
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                }
            }

            #region Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}