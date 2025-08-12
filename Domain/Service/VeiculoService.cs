using MinimalApi.Domain.Entity;
using MinimalApi.Infrastructure.Interface;
using MinimalApi.Infrastructure.Db;
using System.Linq;

namespace MinimalApi.Domain.Service
{
    public class VeiculoService : IVeiculoService
    {
        private readonly AppDbContext _context;
        public VeiculoService(AppDbContext context)
        {
            _context = context;
        }

        public void Atualizar(Veiculo veiculo)
        {
            if (veiculo != null)
            {
                _context.Veiculos.Update(veiculo);
                _context.SaveChanges();
            }
        }

        Veiculo? IVeiculoService.BuscaPorId(int id)
        {
            return _context.Veiculos.FirstOrDefault(v => v.Id == id);
        }

        public void Excluir(int id)
        {
            var veiculo = ((IVeiculoService)this).BuscaPorId(id);
            _context.Veiculos.Remove(veiculo);
            _context.SaveChanges();
        }

        public void Incluir(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            _context.SaveChanges();
        }

        // Alteração da assinatura do método Todos para corresponder à interface IVeiculoService
        public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome != null && v.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca != null && v.Marca.Contains(marca, StringComparison.OrdinalIgnoreCase));
            }

            int pageSize = 10;
            int pageNumber = pagina;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }
    }
}
