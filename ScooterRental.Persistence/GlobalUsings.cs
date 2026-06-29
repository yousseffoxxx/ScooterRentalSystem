global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Logging;
global using NetTopologySuite.Geometries;
global using NetTopologySuite.IO.Converters;
global using ScooterRental.Domain.Enums;
global using ScooterRental.Domain.Models.Auth;
global using ScooterRental.Domain.Models.Payment;
global using ScooterRental.Domain.Models.Rides;
global using ScooterRental.Domain.Models.Scooters;
global using ScooterRental.MqttService.Abstractions;
global using ScooterRental.Persistence.Data.Contexts;
global using ScooterRental.Service.Abstractions;
global using ScooterRental.Service.Abstractions.RepositoryContracts;
global using ScooterRental.Service.Abstractions.Specifications;
global using ScooterRental.Shared.Events;
global using StackExchange.Redis;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Microsoft.Extensions.Hosting;
global using ScooterRental.Shared;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using ScooterRental.Persistence.Repositories;





















