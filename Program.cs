using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Entity;
using MinimalApi.Domain.Enuns;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Service;
using MinimalApi.Infrastructure.Db;
using MinimalApi.Infrastructure.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

#region Builder

var builder = WebApplication.CreateBuilder(args);

// Configuração JWT
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(key))
    key = "123456";

// Serviços básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minha API",
        Version = "v1"
    });

    // Suporte para tipos não anuláveis
    options.SupportNonNullableReferenceTypes();
});

// Serviços da aplicação

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
       ValidateLifetime = true,
       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});


builder.Services.AddAuthorization();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MyPostgres"));
});

var app = builder.Build();

#endregion

#region Rotas Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores

string GerarTokenJwt(Administrator admin)
{
    if(string.IsNullOrEmpty(key)) 
        return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", admin.Email),
        new Claim("Perfil", admin.Perfil.ToString())
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    var adm = adminService.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).WithTags("Administrador");

app.MapPost("/CadastrarUsuário", ([FromBody] AdministratorDTO adminDTO, IAdminService adminService) =>
{
    var validacao = new ErrosDeValidacaoAdmin { Mensagens = new List<string>() };

    if (string.IsNullOrEmpty(adminDTO.Name) || adminDTO.Name == "string")
        validacao.Mensagens.Add("O nome do usuário é obrigatório.");
    if (string.IsNullOrEmpty(adminDTO.Email) || adminDTO.Email == "string")
        validacao.Mensagens.Add("O email do usuário é obrigatório.");
    if (string.IsNullOrEmpty(adminDTO.Password) || adminDTO.Email == "string")
        validacao.Mensagens.Add("A senha do usuário é obrigatória.");
    if (string.IsNullOrEmpty(adminDTO.Perfil.ToString()))
        validacao.Mensagens.Add("O perfil do usuário é obrigatório.");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var administrador = new Administrator
    {
        Name = adminDTO.Name,
        Email = adminDTO.Email,
        Password = adminDTO.Password,
        Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
    };

    return Results.Created($"/Administradores/{adminService.Incluir(administrador).Id}", administrador);

}).RequireAuthorization().WithTags("Administrador");

app.MapPost("/ConsultaListaUsuários", ([FromQuery] int? pagina, IAdminService adminService) =>
{
    return Results.Ok(adminService.Todos(pagina ?? 1));
}).RequireAuthorization().WithTags("Administrador");

app.MapGet("/BuscarUsuarioPorId/{id}", ([FromRoute] int id, IAdminService adminiService) =>
{
    var admin = adminiService.BuscarPorId(id);
    if (admin == null) return Results.NotFound();

    return Results.Ok(new AdminModelView
    {
        Id = admin.Id,
        Name = admin.Name,
        Email = admin.Email
    });

}).RequireAuthorization().WithTags("Administrador");
#endregion

#region Veículos
ValidationError validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ValidationError { Mensagens = new List<string>() };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome do veículo é obrigatório.");
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("É necessário o preenchimento da marca do veículo");
    if (veiculoDTO.Ano < 1945)
        validacao.Mensagens.Add("É necessário preencher o ano do veículo corretamente");

    return validacao;
}

app.MapPost("/AdicionarVeiculo", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

}).RequireAuthorization().WithTags("Veículo");

app.MapGet("/ConsultarRelaçãoVeiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.Todos(pagina ?? 1);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículo");

app.MapGet("/ConsultarVeiculoPorId/{id:int}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.BuscaPorId(id);
    if (veiculo == null)
        return Results.NotFound();

    var veiculoDTO = new VeiculoDTO
    {
        Nome = veiculo.Nome,
        Marca = veiculo.Marca,
        Ano = veiculo.Ano
    };
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veículo");

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
}).RequireAuthorization().WithTags("Veículo");

app.MapDelete("/ExcluirVeiculo/{id:int}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.BuscaPorId(id);
    if (veiculo == null)
        return Results.NotFound();

    veiculoService.Excluir(id);
    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veículo");
#endregion

#region Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion