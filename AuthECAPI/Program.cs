using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Config. CORS
app.UseCors(options =>
options.WithOrigins("http://localhost:4200")
.AllowAnyMethod()
.AllowAnyHeader());
#endregion

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

app.Run();


public class UserRegistrationModel
{
    //public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}