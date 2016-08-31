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
    builder.EntitySet<supplier>("suppliers");
    builder.EntitySet<account>("accounts"); 
    builder.EntitySet<loan>("loans"); 
    builder.EntitySet<product>("products"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class suppliersController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/suppliers
        [EnableQuery]
        public IQueryable<supplier> Getsuppliers()
        {
            return db.suppliers;
        }

        // GET: odata/suppliers(5)
        [EnableQuery]
        public SingleResult<supplier> Getsupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.suppliers.Where(supplier => supplier.SupplierID == key));
        }

        // PUT: odata/suppliers(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<supplier> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            supplier supplier = db.suppliers.Find(key);
            if (supplier == null)
            {
                return NotFound();
            }

            patch.Put(supplier);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!supplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplier);
        }

        // POST: odata/suppliers
        public IHttpActionResult Post(supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.suppliers.Add(supplier);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (supplierExists(supplier.SupplierID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(supplier);
        }

        // PATCH: odata/suppliers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<supplier> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            supplier supplier = db.suppliers.Find(key);
            if (supplier == null)
            {
                return NotFound();
            }

            patch.Patch(supplier);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!supplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplier);
        }

        // DELETE: odata/suppliers(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            supplier supplier = db.suppliers.Find(key);
            if (supplier == null)
            {
                return NotFound();
            }

            db.suppliers.Remove(supplier);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/suppliers(5)/accounts
        [EnableQuery]
        public IQueryable<account> Getaccounts([FromODataUri] int key)
        {
            return db.suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.accounts);
        }

        // GET: odata/suppliers(5)/loans
        [EnableQuery]
        public IQueryable<loan> Getloans([FromODataUri] int key)
        {
            return db.suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.loans);
        }

        // GET: odata/suppliers(5)/products
        [EnableQuery]
        public IQueryable<product> Getproducts([FromODataUri] int key)
        {
            return db.suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.products);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool supplierExists(int key)
        {
            return db.suppliers.Count(e => e.SupplierID == key) > 0;
        }
    }
}
