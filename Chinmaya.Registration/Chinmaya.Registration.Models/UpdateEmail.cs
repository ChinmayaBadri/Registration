using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Chinmaya.Registration.Models
{
	public class UpdateEmail
	{
		public string userId { get; set; }
		//[Required]
		[DisplayName("Email")]
		[RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
		[Required(ErrorMessage = "Please enter your e-mail address.")]
		//[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }
	}
}
