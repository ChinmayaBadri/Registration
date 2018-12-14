using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class MainEventModel
	{
		public List<CurrentEventModel> currentEventModel { get; set; }
		public EventsModel events { get; set; }
		
		public List<Weekdays> weekday { get; set; }
		public List<Frequencies> frequencies { get; set; }
		public List<Sessions> sessions { get; set; }

		public UpdatePasswordModel updatePasswordModel { get; set; } = new UpdatePasswordModel();
		public UpdatePhone updatePhone { get; set; } = new UpdatePhone();
		public UpdateEmail updateMailModel { get; set; } = new UpdateEmail();
		public ContactDetails updateAddressModel { get; set; } = new ContactDetails();

	}

	public class Weekdays
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Nullable<int> FrequencyId { get; set; }
	}
	public class Frequencies
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	public class Sessions
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
	}
}

