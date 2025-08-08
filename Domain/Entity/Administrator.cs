using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entity
{
    public class Administrator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        public string Password { get; set; }

        [StringLength(50)]
        public string Perfil { get; set; }
        public Administrator(int id, string name, string email, string password, string perfil)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Perfil = perfil;
        }
        public Administrator() { }
    }
}
