using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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