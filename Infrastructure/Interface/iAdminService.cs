using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Entity;

namespace MinimalApi.Infrastructure.Interface
{
    public interface iAdminService
    {
        Administrator? Login(LoginDTO login);
    }
}
