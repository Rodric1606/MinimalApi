using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Service;
using MinimalApi.Infrastructure.Db;
using MinimalApi.Infrastructure.Interface;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<iAdminService, AdminService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MyPostgres"));
});

var app = builder.Build();

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/login", ([FromBody] LoginDTO login, iAdminService adminService) => { 
    if(adminService.Login(login) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
}).WithTags("Administrador");
#endregion

#region Veículos
app.MapPost("/Adicionar veiculo", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veículo");

app.MapGet("/ConsultarRelaçãoVeiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.Todos(pagina ?? 1);
    return Results.Ok(veiculos);
}).WithTags("Veículo");

app.MapGet("/ConsultarVeiculoPorId/{id:int}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.BuscaPorId(id);
    if (veiculo == null)
        return Results.NotFound();
    return Results.Ok(veiculo);
}).WithTags("Veículo");

app.MapPut("/AtualizarVeiculo/{id:int}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.BuscaPorId(id); 
    if (veiculo == null)
        return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;
    veiculoService.Atualizar(veiculo);

    return Results.Ok(veiculo);

}).WithTags("Veículo");

app.MapDelete("/ExcluirVeiculo/{id:int}", ([FromRoute] int id, VeiculoDTO veiculoDTo,  IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.BuscaPorId(id);
    if (veiculo == null)
        return Results.NotFound();

    veiculoService.Excluir(id);

    return Results.Ok(veiculo);

}).WithTags("Veículo");
#endregion

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
