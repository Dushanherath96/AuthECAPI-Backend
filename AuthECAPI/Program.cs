using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//InjectDbContext is extension method for inject AppDbContext with SQL Server
//AddIdentityHandlersAndStores is extension method for add IdentityApiEndpoints and EntityFrameworkStores
//ConfigureIdentityOptions is extension method for configure Identity options
//AddIdentityAuth is extension method for add Authentication with JWT Bearer
builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerExplorer()
   .ConfigCORS(builder.Configuration)
   .AddIdentityAuthMiddleware();

app.MapControllers();
app.MapGroup("/api")
   .MapIdentityApi<AppUser>();

app.MapGroup("/api")
   .MapIdentityUserEndpoints();

app.Run();