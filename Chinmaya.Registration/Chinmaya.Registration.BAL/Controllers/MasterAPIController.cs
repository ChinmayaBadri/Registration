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
    public class MasterAPIController : ApiController
    {
        Master _master = new Master();

        [HttpPost]
        [ResponseType(typeof(List<KeyValueModel>))]
        public IHttpActionResult GetMasterData([FromBody]Utility.MasterType masterValue)
        {            
            return Ok(_master.GetMasterData(masterValue));
        }
    }
}
