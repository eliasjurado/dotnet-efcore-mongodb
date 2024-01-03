namespace BookStore.Models.Dto
{
    public record BookDto
    {
        public string Name { get; set; } = string.Empty;
        public int Pages { get; set; }
    }
}
