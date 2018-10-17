using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Chinmaya.Registration.Models;
using Chinmaya.Registration.DAL;

namespace Chinmaya.Registration.BAL.Controllers
{
    public class EmployeeInfoAPIController : ApiController
    {
        Employee _emp = new Employee();
        public IEnumerable<EmployeeInfo> Get()
        {
            return _emp.Get();
        }

        [ResponseType(typeof(EmployeeInfo))]
        public IHttpActionResult Get(int id)
        {
            return Ok(_emp.Get(id));
        }

        [ResponseType(typeof(EmployeeInfo))]
        public IHttpActionResult Post(EmployeeInfo emp)
        {
            _emp.Post(emp);
            return Ok(emp);
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult Put(int id, EmployeeInfo emp)
        {
            _emp.Put(id, emp);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult Delete(int id)
        {
            _emp.Delete(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
