using Microsoft.EntityFrameworkCore;
using ShiftPro.Data;
using ShiftPro.Helpers;
using ShiftPro.Interfaces;
using ShiftPro.Services.Employees;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
