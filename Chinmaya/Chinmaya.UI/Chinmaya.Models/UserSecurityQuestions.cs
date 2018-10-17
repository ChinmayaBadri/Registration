using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Models
{
	public class UserSecurityQuestions
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int SecurityQuestionId { get; set; }
		public string Answer { get; set; }
	}
}
