using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using AutoMapper;
using System.Data.Entity.Validation;

namespace Chinmaya.Registration.DAL
{
    public class Payments
    {
        /// <summary>
        /// saves check payment details 
        /// </summary>
        /// <param name="chkp"> Check Payment Model </param>
        public void PostCheckPayment(CheckPaymentModel chkp)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CheckPaymentModel, CheckPayment>();
                });
                IMapper mapper = config.CreateMapper();

                var chk = new CheckPayment
                {
                    AccountHolderName = chkp.AccountHolderName,
                    AccountTypeId = chkp.AccountTypeId,
                    BankName = chkp.BankName,
                    AccountNumber = chkp.AccountNumber,
                    RoutingNumber = chkp.RoutingNumber,
                    Amount = chkp.Amount,
                    StatusId = 1,
                    CreatedBy = chkp.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                _ctx.CheckPayments.Add(chk);
                try
                {
                    _ctx.SaveChanges();
                }
				catch
				{
					throw;
				}
			}
        }

		/// <summary>
		/// saves Event Registration details 
		/// </summary>
		/// <param name="evntr"> Event Registration Model </param>
		public void PostEventRegistration(EventRegistrationModel evntr)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<EventRegistrationModel, EventRegistration>();
				});
				IMapper mapper = config.CreateMapper();

				var eventRegistration = new EventRegistration();
					eventRegistration.EventId = evntr.EventId;
					eventRegistration.FamilyMemberId = evntr.FamilyMemberId;
					eventRegistration.OwnerId = evntr.OwnerId;
					eventRegistration.CheckPaymentId = _ctx.CheckPayments.Where(r => r.CreatedBy == evntr.OwnerId).Select(n => n.Id).FirstOrDefault();
					eventRegistration.InvoiceId = _ctx.CreditCardPayments.Where(r => r.UserId == evntr.OwnerId).Select(n => n.InvoiceId).FirstOrDefault();
					eventRegistration.IsRegister = true;
					eventRegistration.IsConfirm = true;
					eventRegistration.IsPaid = true;
					eventRegistration.CreatedBy = evntr.OwnerId;
					eventRegistration.CreatedDate = DateTime.Now;

				_ctx.EventRegistrations.Add(eventRegistration);
				try
				{
					_ctx.SaveChanges();
				}
				catch
				{
					throw;
				}
			}
		}
	}
}
