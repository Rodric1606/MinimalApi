using MinimalApi.Infrastructure.Interface;
using MinimalApi.Domain.DTO;
using MinimalApi.Domain.Entity;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Service
{
    public class AdminService : iAdminService
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
    }
}
