using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Chinmaya.Registration.Utilities;
using System.Net.Http;
using System.Web.Configuration;

namespace Chinmaya.Registration.Models
{
	public class AccountDetails
	{

		[Required]
		[DisplayName("Email")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[StringLength(256, ErrorMessage = "Email should not be more than 256 characters")]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Passwords must have combination of atleast 1 Capital Letter, 1 Number, one Special Symbol($, #, @, !, %) and minimum length of 8. ")]
		[DisplayName("Password")]
		public string Password { get; set; }
		[Required]
		[DisplayName("Retype Password")]
		[DataType(DataType.Password)]
		[Compare("Password")]
		public string RetypePassword { get; set; }
		[DisplayName("Account Type")]
		public int AccountType { get; set; }

		
		[DisplayName("Security Questions")]
		public List<SecurityQuestionsModel> SecurityQuestionsModel { get; set; }

		
	}
}
