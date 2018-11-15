using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class ProgramEventRegistrationModel
	{
		//public string UserId { get; set; }
		//public string FirstName { get; set; }
		//public string LastName { get; set; }
		public List<UserFamilyMember> uFamilyMembers { get; set; }
		public IEnumerable<CurrentEventModel> Events { get; set; }
	}
}
