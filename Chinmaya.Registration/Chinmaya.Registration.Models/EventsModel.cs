﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chinmaya.Registration.Models
{
	public class EventsModel
	{
		public string Id { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Name should not exceed 100 characters")]
		[DisplayName("Name")]
		public string Name { get; set; }

		[StringLength(1000, ErrorMessage = "Description should not exceed 1000 characters")]
		public string Description { get; set; }

		[DisplayName("Weekday")]
		public Nullable<int> WeekdayId { get; set; }
		public string WeekdayName { get; set; }

		[Required]
		[DisplayName("Frequency")]
		public int FrequencyId { get; set; }

		public Nullable<int> AgeFrom { get; set; }
		public Nullable<int> AgeTo { get; set; }
		public Nullable<decimal> Amount { get; set; }
		public bool Status { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<System.DateTime> UpdatedDate { get; set; }


		
		public string EventId { get; set; }

		[Required]
		[DisplayName("Session")]
		public int SessionId { get; set; }
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[DataType(DataType.Date)]
        [DisplayName("Start Date")]
        public Nullable<System.DateTime> StartDate { get; set; }
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[DataType(DataType.Date)]
        [DisplayName("End Date")]
        public Nullable<System.DateTime> EndDate { get; set; }
		[Required]
		[DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan StartTime { get; set; }
		[Required]
		[DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan EndTime { get; set; }

		[StringLength(300, ErrorMessage = "Location should not exceed 300 characters")]
		public string Location { get; set; }
		[StringLength(300, ErrorMessage = "Instructor Name should not exceed 300 characters")]
		public string Instructor { get; set; }
		[StringLength(300, ErrorMessage = "Contact details should not exceed 300 characters")]
		public string Contact { get; set; }
		[StringLength(300, ErrorMessage = "Other information should not exceed 300 characters")]
		public string Other { get; set; }

		[Required]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Holiday Date")]
        public System.DateTime HolidayDate { get; set; }

		public List<Weekdays> weekday { get; set; }
		public List<Frequencies> frequencies { get; set; }
		public List<Sessions> sessions { get; set; }
	}
}
