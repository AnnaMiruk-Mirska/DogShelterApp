using System.ComponentModel.DataAnnotations;


namespace TestApp.Models
{
    public class LoginViewModel
    {
        [Key]
        public int id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Name")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string Password { get; set; }
    }
}