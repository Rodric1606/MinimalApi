using MinimalApi.Infrastructure.Interface;
using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Entity;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Service
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _db;
        public AdminService(AppDbContext db)
        {
            _db = db;
        }

        public Administrator? Login(LoginDTO login)
        {
            var admin = (_db.Administrators.Where(a => a.Email == login.Email && a.Password == login.Password).FirstOrDefault());
            return admin;
        }

        // Corrigido para usar Administrator em vez de AdministratorDTO
        public Administrator Incluir(Administrator admin)
        {
            _db.Administrators.Add(admin);
            _db.SaveChanges();

            return admin;
        }

        public List<Administrator> Todos(int pagina = 1, string? email = null, string? perfil = null)
        {
            var query = _db.Administrators.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(a => a.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(perfil))
            {
                query = query.Where(a => a.Perfil == perfil);
            }

            int pageSize = 10;
            int pageNumber = pagina;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }
        public Administrator BuscarPorId(int id)
        {
            return _db.Administrators.FirstOrDefault(a => a.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        }
    }
}
