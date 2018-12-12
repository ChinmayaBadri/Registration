using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Chinmaya.Registration.Models
{
    public class UpdatePasswordModel
    {
        public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Passwords must have combination of atleast 1 Capital Letter, 1 Number, one Special Symbol($, #, @, !, %) and minimum length of 8. ")]
		[DisplayName("Old Password")]
		public string OldPassword { get; set; }

		[Required]
		//[NotEqualTo("OldPassword", ErrorMessage = "New Password should not match with the Old Password.")]
		[DataType(DataType.Password)]
		[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Passwords must have combination of atleast 1 Capital Letter, 1 Number, one Special Symbol($, #, @, !, %) and minimum length of 8. ")]
		[DisplayName("New Password")]
		public string NewPassword { get; set; }

		[Required]
		[DisplayName("Confirm New Password")]
		[DataType(DataType.Password)]
		[Compare("NewPassword")]
		public string RetypePassword { get; set; }
	}
}
