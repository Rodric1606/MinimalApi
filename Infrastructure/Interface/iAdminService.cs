using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Entity;

namespace MinimalApi.Infrastructure.Interface
{
    public interface IAdminService
    {
        Administrator? Login(LoginDTO login);
        Administrator Incluir(Administrator admin);
        List<Administrator> Todos(int pagina = 1, string? email = null, string? perfil = null);
        public Administrator BuscarPorId(int id);    
    }
}
