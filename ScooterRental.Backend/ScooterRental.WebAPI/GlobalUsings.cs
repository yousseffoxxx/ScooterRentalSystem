global using FluentValidation;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Scalar.AspNetCore;
global using ScooterRental.Domain.Exceptions;
global using ScooterRental.Domain.Models.Auth;
global using ScooterRental.MqttService.Abstractions;
global using ScooterRental.Persistence.Data.Contexts;
global using ScooterRental.Persistence.Repositories;
global using ScooterRental.Service;
global using ScooterRental.Service.Abstractions;
global using ScooterRental.Service.Abstractions.AuthServices;
global using ScooterRental.Service.Abstractions.RepositoryContracts;
global using ScooterRental.Service.AuthServices;
global using ScooterRental.Service.EmailServices;
global using ScooterRental.Service.Validations.Auth;
global using ScooterRental.Shared;
global using ScooterRental.Shared.ErrorModels;
global using ScooterRental.WebAPI.CustomMiddleware;
global using Serilog;
global using StackExchange.Redis;
global using System.Text;
global using ILogger = Microsoft.Extensions.Logging.ILogger;












