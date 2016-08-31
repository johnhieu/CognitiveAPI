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
    builder.EntitySet<bank>("banks");
    builder.EntitySet<loan>("loans"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class banksController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/banks
        [EnableQuery]
        public IQueryable<bank> Getbanks()
        {
            return db.banks;
        }

        // GET: odata/banks(5)
        [EnableQuery]
        public SingleResult<bank> Getbank([FromODataUri] int key)
        {
            return SingleResult.Create(db.banks.Where(bank => bank.BankNum == key));
        }

        // PUT: odata/banks(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<bank> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bank bank = db.banks.Find(key);
            if (bank == null)
            {
                return NotFound();
            }

            patch.Put(bank);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!bankExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(bank);
        }

        // POST: odata/banks
        public IHttpActionResult Post(bank bank)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.banks.Add(bank);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (bankExists(bank.BankNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(bank);
        }

        // PATCH: odata/banks(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<bank> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bank bank = db.banks.Find(key);
            if (bank == null)
            {
                return NotFound();
            }

            patch.Patch(bank);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!bankExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(bank);
        }

        // DELETE: odata/banks(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            bank bank = db.banks.Find(key);
            if (bank == null)
            {
                return NotFound();
            }

            db.banks.Remove(bank);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/banks(5)/loans
        [EnableQuery]
        public IQueryable<loan> Getloans([FromODataUri] int key)
        {
            return db.banks.Where(m => m.BankNum == key).SelectMany(m => m.loans);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool bankExists(int key)
        {
            return db.banks.Count(e => e.BankNum == key) > 0;
        }
    }
}
