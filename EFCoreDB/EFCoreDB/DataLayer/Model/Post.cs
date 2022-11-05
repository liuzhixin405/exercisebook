using System.ComponentModel.DataAnnotations;

namespace EFCoreDB.DataLayer.Model
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Blog Blog { get; set; }
    }
}
