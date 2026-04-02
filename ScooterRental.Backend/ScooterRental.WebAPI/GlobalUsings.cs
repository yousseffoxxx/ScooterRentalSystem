global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using ScooterRental.Domain.Models;
global using ScooterRental.Persistence.Data.Contexts;
global using FluentValidation;
global using ScooterRental.Service.Validations;
global using ScooterRental.Shared;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using System.Text;
global using Scalar.AspNetCore;
global using ScooterRental.Domain.Exceptions;
global using ScooterRental.Shared.ErrorModels;
global using ScooterRental.WebAPI.CustomMiddleware;
global using Serilog;
global using ILogger = Microsoft.Extensions.Logging.ILogger;
global using ScooterRental.Service.Abstractions;
global using ScooterRental.Service;
global using ScooterRental.Service.Abstractions.AuthServices;
global using ScooterRental.Service.EmailServices;
global using ScooterRental.Service.AuthServices;










