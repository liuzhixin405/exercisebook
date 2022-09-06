using System.ComponentModel.DataAnnotations;

namespace AbpVuenetcore.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}