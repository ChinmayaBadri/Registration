using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chinmaya.Registration.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="User Name")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
