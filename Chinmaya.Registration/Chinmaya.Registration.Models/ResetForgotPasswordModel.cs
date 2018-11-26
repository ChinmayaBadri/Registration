using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
    public class ResetForgotPasswordModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm password not matched.")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Security Questions")]
        public List<SecurityQuestionsModel> SecurityQuestionsModel { get; set; }
        public bool IsRedirected { get; set; }
    }
}
