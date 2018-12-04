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
                catch (DbEntityValidationException e)
                {
                    foreach (var even in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            even.Entry.Entity.GetType().Name, even.Entry.State);
                        foreach (var ve in even.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                }
            }
        }
    }
}
