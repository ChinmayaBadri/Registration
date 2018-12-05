using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using Chinmaya.Registration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Chinmaya.Registration.BAL.Controllers
{
    public class MasterController : ApiController
    {
        Master _master = new Master();

        /// <summary>
        /// gets list of key value model by using master value
        /// </summary>
        /// <param name="masterValue"> master value type </param>
        /// <returns> key value model </returns>
        [HttpPost]
        [ResponseType(typeof(List<KeyValueModel>))]
        public IHttpActionResult GetMasterData([FromBody]Utility.MasterType masterValue)
        {            
            return Ok(_master.GetMasterData(masterValue));
        }
    }
}
