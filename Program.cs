using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShiftPro.Data;
using ShiftPro.Helpers;
using ShiftPro.Interfaces;
using ShiftPro.Services.Auth;
using ShiftPro.Services.Employees;
using ShiftPro.Services.Holidays;
using ShiftPro.Services.JWT;
using ShiftPro.Services.Schedules;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DB連線
var connectionString = builder.Configuration.GetConnectionString("ShiftPro");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

//註冊 FileLogger
builder.Services.AddSingleton<FileLogger>();

//註冊 EmployeeService
builder.Services.AddScoped<IEmployeeService, EmployeeService>();


//註冊  ScheduleService
builder.Services.AddScoped<IScheduleService, ScheduleService>();

//註冊  HolidayService
builder.Services.AddScoped<IHolidayService, HolidayService>();

//註冊  AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

//註冊 JwtService
builder.Services.AddScoped<JwtService>();

//jwt 驗證
var jwtKey = builder.Configuration["JWT:Key"];

builder.Services.AddAuthentication(options =>
{
    //預設用哪種方式辨認使用者
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new
        TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://shift-pro-frontend.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
