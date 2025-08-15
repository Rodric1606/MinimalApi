using MinimalApi.Domain.Enuns;

namespace MinimalApi.Domain.DTO
{
    public class AdministratorDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Perfil Perfil { get; set; }
        public AdministratorDTO() { }
        public AdministratorDTO(string email, string password, string perfil)
        {
            Email = email;
            Password = password;
            Perfil = Enum.TryParse<Perfil>(perfil, out var parsedPerfil) ? parsedPerfil : throw new ArgumentException("Perfil inválido", nameof(perfil));
        }
    }
}
