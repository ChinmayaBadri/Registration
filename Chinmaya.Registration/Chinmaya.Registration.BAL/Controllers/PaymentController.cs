using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;

namespace Chinmaya.BAL.Controllers
{
    public class PaymentController : ApiController
    {
        Payments _payment = new Payments();

        /// <summary>
        /// saves check payment details 
        /// </summary>
        /// <param name="obj"> Check Payment Model </param>
        [Route("api/Payment/PostCheckPayment")]
        [HttpPost]
        public IHttpActionResult PostCheckPayment(CheckPaymentModel obj)
        {
            try
            {
                _payment.PostCheckPayment(obj);
                return Ok("Success");
            }
            catch (Exception)
            {
                return Ok("Something went wrong");
            }
        }
    }
}
