# STAGE 1: The Runtime Base (The final, tiny container that will run in the cloud)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Expose the standard cloud ports
EXPOSE 8080
EXPOSE 8081

# STAGE 2: The Build Environment (The heavy SDK container to compile the code)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the entire solution and project files
# (This assumes your Dockerfile is next to your .sln file)
COPY . .

# Restore NuGet packages for the Web API project
# Replace "ScooterRental.WebAPI" with the exact folder/name of your API project if different
RUN dotnet restore "ScooterRental.WebAPI/ScooterRental.WebAPI.csproj"

# Build the project in Release mode
RUN dotnet build "ScooterRental.WebAPI/ScooterRental.WebAPI.csproj" -c Release -o /app/build

# STAGE 3: Publish (Strip out unnecessary files and compress)
FROM build AS publish
RUN dotnet publish "ScooterRental.WebAPI/ScooterRental.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 4: Final Assembly (Put the compiled app into the tiny Runtime Base)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Tell Docker how to start your application
ENTRYPOINT ["dotnet", "ScooterRental.WebAPI.dll"]