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

namespace CognitiveDecisionSystemAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using CognitiveDecisionSystemAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<sale>("sales");
    builder.EntitySet<customer>("customers"); 
    builder.EntitySet<employee>("employees"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class salesController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/sales
        [EnableQuery]
        public IQueryable<sale> Getsales()
        {
            return db.sales;
        }

        // GET: odata/sales(5)
        [EnableQuery]
        public SingleResult<sale> Getsale([FromODataUri] int key)
        {
            return SingleResult.Create(db.sales.Where(sale => sale.SaleID == key));
        }

        // PUT: odata/sales(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<sale> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            sale sale = db.sales.Find(key);
            if (sale == null)
            {
                return NotFound();
            }

            patch.Put(sale);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!saleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sale);
        }

        // POST: odata/sales
        public IHttpActionResult Post(sale sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.sales.Add(sale);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (saleExists(sale.SaleID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(sale);
        }

        // PATCH: odata/sales(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<sale> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            sale sale = db.sales.Find(key);
            if (sale == null)
            {
                return NotFound();
            }

            patch.Patch(sale);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!saleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sale);
        }

        // DELETE: odata/sales(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            sale sale = db.sales.Find(key);
            if (sale == null)
            {
                return NotFound();
            }

            db.sales.Remove(sale);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/sales(5)/customer
        [EnableQuery]
        public SingleResult<customer> Getcustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.sales.Where(m => m.SaleID == key).Select(m => m.customer));
        }

        // GET: odata/sales(5)/employee
        [EnableQuery]
        public SingleResult<employee> Getemployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.sales.Where(m => m.SaleID == key).Select(m => m.employee));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool saleExists(int key)
        {
            return db.sales.Count(e => e.SaleID == key) > 0;
        }
    }
}
