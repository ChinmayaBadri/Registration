using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class ClassesConfirmModel
	{

		public UserFamilyMember uFamilyMembers { get; set; }
		public List<CurrentEventModel> Events { get; set; }
	}

	public class SelectedListModel
	{
		public UserFamilyMember uFamilyMembers { get; set; }
		public List<CurrentEventModel> Events { get; set; }
	}
	
}
