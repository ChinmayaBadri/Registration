using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class MyAccountModel
	{
		public List<UserFamilyMember> userFamilyMember { get; set; }
		public FamilyMemberModel familyMemberModel { get; set; }


		public List<Relationships> relationships { get; set; }
		public List<Grades> grades { get; set; }
		public List<Genders> genders { get; set; }

	}
	public class Relationships
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class Grades
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	public class Genders
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
