using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreDBRelation.DataLayer.Model
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }
        public string Url { get; set; }
        public List<Post> Posts { get; set; }
    }
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Blog Blog { get; set; }

        public List<PostItem> PostItem { get; set; }
    }

    public class PostItem
    {
        [Key]
        public int PostItemId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public Post Post { get; set; }
    }//可以无限嵌套,dbset只用指定Blog一个实体
}
