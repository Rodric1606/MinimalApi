using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Service;
using MinimalApi.Infrastructure.Db;
using MinimalApi.Infrastructure.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<iAdminService, AdminService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MyPostgres"));
});

var app = builder.Build();


app.MapPost("/login", ([FromBody] LoginDTO login, iAdminService adminService) => { 
    if(adminService.Login(login) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});


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
