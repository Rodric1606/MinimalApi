using MinimalApi.Domain.Entity;

namespace MinimalApi.Infrastructure.Interface
{
    public interface IVeiculoService
    {
        List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo? BuscaPorId(int id);
        public void Incluir(Veiculo veiculo);  
        public void Atualizar(Veiculo veiculo);
        public void Excluir(int id);   
    }
}
