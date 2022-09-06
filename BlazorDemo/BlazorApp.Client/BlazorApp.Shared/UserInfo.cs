using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared
{
    public class UserInfo
    {
        public int DeptId { get; set; }
        public int UserID { get; set; }
        [Display(Name="姓名")]
        [Required(ErrorMessage ="{0}是必填项")]
        [MaxLength(50,ErrorMessage ="{0}的长度不能超过{1}")]
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string PictureUrl { get; set; }
    }
}
