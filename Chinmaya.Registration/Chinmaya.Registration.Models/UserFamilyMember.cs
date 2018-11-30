using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class UserFamilyMember
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public System.DateTime DOB { get; set; }
		public string Relationship { get; set; }
		public string Grade { get; set; }

		public List<CurrentEventModel> Events { get; set; }

	}

	public class UFamilyMember
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DOB { get; set; }
		public string Relationship { get; set; }
		public string Grade { get; set; }
		public string Fullname { get; set; }

		public List<CurrentEventModel> Events { get; set; }
	}
}
