namespace webapi
{
    public record struct ProductRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreateDateTime { get; set; }
    }
}
