global using FirebaseAdmin.Auth;
global using FirebaseAdmin.Messaging;
global using FluentValidation;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using MQTTnet;
global using MQTTnet.Protocol;
global using NetTopologySuite.Algorithm;
global using NetTopologySuite.Geometries;
global using ScooterRental.Domain.Enums;
global using ScooterRental.Domain.Exceptions;
global using ScooterRental.Domain.Models.Auth;
global using ScooterRental.Domain.Models.Payment;
global using ScooterRental.Domain.Models.Rides;
global using ScooterRental.Domain.Models.Scooters;
global using ScooterRental.MqttService.Abstractions;
global using ScooterRental.Service.Abstractions;
global using ScooterRental.Service.Abstractions.AuthServices;
global using ScooterRental.Service.Abstractions.NotificationServices;
global using ScooterRental.Service.Abstractions.PaymentServices;
global using ScooterRental.Service.Abstractions.RepositoryContracts;
global using ScooterRental.Service.Abstractions.RideServices;
global using ScooterRental.Service.Abstractions.ScooterServices;
global using ScooterRental.Service.Abstractions.Specifications;
global using ScooterRental.Service.Abstractions.StorageServices;
global using ScooterRental.Service.Abstractions.TariffServices;
global using ScooterRental.Service.Abstractions.ZoneServices;
global using ScooterRental.Service.AuthServices;
global using ScooterRental.Service.Mappings;
global using ScooterRental.Service.MqttServices;
global using ScooterRental.Service.PaymentServices;
global using ScooterRental.Service.RideServices;
global using ScooterRental.Service.ScooterServices;
global using ScooterRental.Service.Specifications;
global using ScooterRental.Service.TariffServices;
global using ScooterRental.Service.ZoneServices;
global using ScooterRental.Shared;
global using ScooterRental.Shared.DTOs.AdminManagement.Payment.Request;
global using ScooterRental.Shared.DTOs.AdminManagement.Payment.Response;
global using ScooterRental.Shared.DTOs.AdminManagement.Users.Request;
global using ScooterRental.Shared.DTOs.AdminManagement.Users.Response;
global using ScooterRental.Shared.DTOs.Auth.Request;
global using ScooterRental.Shared.DTOs.Auth.Response;
global using ScooterRental.Shared.DTOs.Payment.Response;
global using ScooterRental.Shared.DTOs.Ride.Request;
global using ScooterRental.Shared.DTOs.Ride.Response;
global using ScooterRental.Shared.DTOs.Scooter.Request;
global using ScooterRental.Shared.DTOs.Scooter.Response;
global using ScooterRental.Shared.DTOs.Tariff.Request;
global using ScooterRental.Shared.DTOs.Tariff.Response;
global using ScooterRental.Shared.DTOs.Zone.Request;
global using ScooterRental.Shared.DTOs.Zone.Response;
global using ScooterRental.Shared.Events;
global using SendGrid;
global using SendGrid.Helpers.Mail;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq.Expressions;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using ScooterRental.Service.Abstractions.RealTimeServices;























