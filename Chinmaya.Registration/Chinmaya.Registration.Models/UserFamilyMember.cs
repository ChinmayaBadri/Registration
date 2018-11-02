using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Models
{
	public class UserFamilyMember
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public System.DateTime DOB { get; set; }
		public int RelationshipId { get; set; }
		public string Relationship { get; set; }
		public Nullable<int> GradeId { get; set; }
		public string Grade { get; set; }
		public string UserId { get; set; }
		public string UserFirstName { get; set; }
		public string UserLastName { get; set; }
		public Nullable<System.DateTime> UserDOB { get; set; }
	}
}
