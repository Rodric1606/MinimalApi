namespace MinimalApi.Domain.ModelViews
{
    public record AdminModelView
    {
        public int Id { get; set; }
        public string Name { get; set;  }
        public string Email { get; set; } = default!;

    }
}
