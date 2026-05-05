namespace ScooterRental.Persistence
{
    public class DataSeeder(ApplicationDbContext _dbContext, UserManager<User> _userManager,
    RoleManager<IdentityRole<Guid>> _roleManager, ILogger<DataSeeder> _logger) : IDataSeeder
    {
        public async Task DataSeedAsync()
        {
            try
            {
                var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = 
                    { 
                        new GeoJsonConverterFactory(geometryFactory),
                        new JsonStringEnumConverter()
                    }
                };
                
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                    await _dbContext.Database.MigrateAsync();

                var basePath = Path.Combine(AppContext.BaseDirectory, "Data", "DataSeed");

                if (!await _dbContext.ScooterModels.AnyAsync())
                {
                    var scooterModelsData = File.OpenRead(Path.Combine(basePath, "scooterModels.json"));

                    var scooterModels = await JsonSerializer.DeserializeAsync<List<ScooterModel>>(scooterModelsData);

                    if (scooterModels is not null && scooterModels.Any())
                        await _dbContext.ScooterModels.AddRangeAsync(scooterModels);
                }

                if (!await _dbContext.Scooters.AnyAsync())
                {
                    var scootersData = File.OpenRead(Path.Combine(basePath, "scooters.json"));

                    var scooters = await JsonSerializer.DeserializeAsync<List<Scooter>>(scootersData, jsonOptions);

                    if (scooters is not null && scooters.Any())
                        await _dbContext.Scooters.AddRangeAsync(scooters);
                }

                if (!await _dbContext.Tariffs.AnyAsync())
                {
                    var tariffsData = File.OpenRead(Path.Combine(basePath, "tariffs.json"));

                    var tariffs = await JsonSerializer.DeserializeAsync<List<Tariff>>(tariffsData);

                    if (tariffs is not null && tariffs.Any())
                        await _dbContext.Tariffs.AddRangeAsync(tariffs);
                }

                if (!await _dbContext.Zones.AnyAsync())
                {
                    var zonesData = File.OpenRead(Path.Combine(basePath, "zones.json"));

                    var zones = await JsonSerializer.DeserializeAsync<List<Zone>>(zonesData, jsonOptions);

                    if (zones is not null && zones.Any())
                        await _dbContext.Zones.AddRangeAsync(zones);
                }

                if (!await _dbContext.Roles.AnyAsync())
                {
                    var roles = new[] { "Admin", "Customer", "Support" };
                    foreach (var role in roles)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                            await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                }

                if (!await _userManager.Users.AnyAsync())
                {
                    var adminUser = new User
                    {
                        FullName = "Youssef Ahmed Admin",
                        Email = "youssef5fox5@gmail.com",
                        EmailConfirmed = true,
                        UserName = "youssef5fox5@gmail.com",
                        PhoneNumber = "01556324346",
                        PhoneNumberConfirmed = true,
                        AccountStatus = AccountStatus.Active,
                        CreatedAt = DateTimeOffset.UtcNow,
                        IdPhotoUrl = "https://dummyimage.com/600x400/000/fff&text=Fake+ID",
                        IdVerificationStatus = IdVerificationStatus.Verified
                    };

                    var result = await _userManager.CreateAsync(adminUser, "P@ssw0rd");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");

                        var wallet = new Wallet
                        {
                            UserId = adminUser.Id,
                            Balance = 100.00m,
                            HeldAmount = 0
                        };
                        await _dbContext.Wallets.AddAsync(wallet);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred during database seeding. Halting startup.");
                throw;
            }
        }
    }
}
