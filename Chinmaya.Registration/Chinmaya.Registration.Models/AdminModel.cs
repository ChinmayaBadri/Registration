using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class AdminModel
	{
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsIndividual { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public DateTime DOB { get; set; }
		public string HomePhone { get; set; }
		public string CellPhone { get; set; }
		//public string AccountType { get; set; }
		public List<UserFamilyMember> FamilyMembers { get; set; }
	}
}
