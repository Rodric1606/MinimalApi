namespace MinimalApi.Domain.DTO
{
    public record VeiculoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }

        public VeiculoDTO() { } 
        public VeiculoDTO(string nome, string marca, int ano)
        {
            Nome = nome;
            Marca = marca;
            Ano = ano;
        }

    }
}
