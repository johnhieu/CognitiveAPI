using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using CognitiveDecisionSystemAPI.Models;
using System.Web.Http.Cors;

namespace CognitiveDecisionSystemAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using CognitiveDecisionSystemAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<employee>("employees");
    builder.EntitySet<sale>("sales"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    // Please look at the comments in the accountsController for more details
    [EnableCors(origins: "http://localhost:52527", headers: "*", methods: "*")]
    public class employeesController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/employees
        [EnableQuery]
        public IQueryable<employee> Getemployees()
        {
            return db.employees;
        }

        // GET: odata/employees(5)
        [EnableQuery]
        public SingleResult<employee> Getemployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.employees.Where(employee => employee.EmployeeId == key));
        }

        // PUT: odata/employees(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<employee> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee employee = db.employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            patch.Put(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(employee);
        }

        // POST: odata/employees
        public IHttpActionResult Post(employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.employees.Add(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (employeeExists(employee.EmployeeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(employee);
        }

        // PATCH: odata/employees(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<employee> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee employee = db.employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            patch.Patch(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(employee);
        }

        // DELETE: odata/employees(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            employee employee = db.employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            db.employees.Remove(employee);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/employees(5)/sales
        [EnableQuery]
        public IQueryable<sale> Getsales([FromODataUri] int key)
        {
            return db.employees.Where(m => m.EmployeeId == key).SelectMany(m => m.sales);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool employeeExists(int key)
        {
            return db.employees.Count(e => e.EmployeeId == key) > 0;
        }
    }
}
