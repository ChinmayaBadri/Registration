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
		[Required]
		public string email { get; set; }

	}
}
