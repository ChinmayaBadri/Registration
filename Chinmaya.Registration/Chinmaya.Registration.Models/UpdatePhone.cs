using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Chinmaya.Registration.Models
{
	public class UpdatePhone
	{
		public string Email { get; set; }

		[RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Phone Number")]
		[DisplayName("Phone")]
		public string OldPhone { get; set; }
	}
}

