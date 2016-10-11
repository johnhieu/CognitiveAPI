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
    builder.EntitySet<customer>("customers");
    builder.EntitySet<account>("accounts"); 
    builder.EntitySet<sale>("sales"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    // Please look at the comments in the accountsController for more details
    [EnableCors(origins: "http://localhost:52527", headers: "*", methods: "*")]
    public class customersController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/customers
        [EnableQuery]
        public IQueryable<customer> Getcustomers()
        {
            return db.customers;
        }

        // GET: odata/customers(5)
        [EnableQuery]
        public SingleResult<customer> Getcustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.customers.Where(customer => customer.CustId == key));
        }

        // PUT: odata/customers(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<customer> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer customer = db.customers.Find(key);
            if (customer == null)
            {
                return NotFound();
            }

            patch.Put(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!customerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customer);
        }

        // POST: odata/customers
        public IHttpActionResult Post(customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.customers.Add(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (customerExists(customer.CustId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(customer);
        }

        // PATCH: odata/customers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<customer> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer customer = db.customers.Find(key);
            if (customer == null)
            {
                return NotFound();
            }

            patch.Patch(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!customerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customer);
        }

        // DELETE: odata/customers(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            customer customer = db.customers.Find(key);
            if (customer == null)
            {
                return NotFound();
            }

            db.customers.Remove(customer);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/customers(5)/accounts
        [EnableQuery]
        public IQueryable<account> Getaccounts([FromODataUri] int key)
        {
            return db.customers.Where(m => m.CustId == key).SelectMany(m => m.accounts);
        }

        // GET: odata/customers(5)/sales
        [EnableQuery]
        public IQueryable<sale> Getsales([FromODataUri] int key)
        {
            return db.customers.Where(m => m.CustId == key).SelectMany(m => m.sales);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool customerExists(int key)
        {
            return db.customers.Count(e => e.CustId == key) > 0;
        }
    }
}
