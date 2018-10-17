using Chinmaya.DAL;
using Chinmaya.Models;
using Chinmaya.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Chinmaya.BAL.Controllers
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
