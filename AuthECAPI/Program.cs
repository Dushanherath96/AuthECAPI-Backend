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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Identity API Endpoints with Entity Framework Stores
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireUppercase = false;

});

// Configure Entity Framework with SQL Server
//pass DbContext to WeatherForecastController via DI
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevDB")));

//addAuthentication use for Identity
builder.Services.AddAuthentication(x => 
{
    //A scheme is a way of identifying the authentication handler to be used.
    x.DefaultAuthenticateScheme =
    x.DefaultChallengeScheme =
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(y => 
{
    y.SaveToken = false;
    y.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey (
            Encoding.UTF8.GetBytes(
                builder.Configuration["AppSettings:JWTSecret"]!))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CORS use for allow request from angular app

#region Config. CORS
app.UseCors(options =>
options.WithOrigins("http://localhost:4200")
.AllowAnyMethod()
.AllowAnyHeader());
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModel userRegistrationModel
    ) => {  
        
        AppUser user = new AppUser
        {
            UserName = userRegistrationModel.Email,
            Email = userRegistrationModel.Email,
            FullName = userRegistrationModel.FullName
        };
        //CreateAsync use for create user with password
        var reuslt = await userManager.CreateAsync(
            user, 
            userRegistrationModel.Password);

        if (reuslt.Succeeded)
            return Results.Ok(reuslt);
        else
            return Results.BadRequest(reuslt);
    });

app.MapPost("/api/signin", async (
    UserManager<AppUser> userManager,
    [FromBody] LoginModel loginModel) =>{
        var user = await userManager.FindByEmailAsync(loginModel.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password)) 
        {
            var signInKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["AppSettings:JWTSecret"]!)
            );
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId",user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(
                    signInKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Results.Ok(new {token});

        } else
            return Results.BadRequest(new { message = "Email or Password is incorrect" });
    });

app.Run();


public class UserRegistrationModel
{
    //public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}

public class LoginModel
{
    //public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}