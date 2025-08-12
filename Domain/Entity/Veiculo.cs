namespace MinimalApi.Domain.Entity
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        public int Ano { get; set; }

        public Veiculo() { }

        public Veiculo(string nome, string marca, int ano)
        {
            Nome = nome;
            Marca = marca;
            Ano = ano;
        }
    }

}
