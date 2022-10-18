namespace DataConsole.Model.BookModel
{
    public class Author
    {
        public int AuthorId { get; set; } 
        public string Name { get; set; }
        
        public string? WebUrl { get; set; } //字段可空
    }
}
