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
    builder.EntitySet<transact>("transacts");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class transactsController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/transacts
        [EnableQuery]
        public IQueryable<transact> Gettransacts()
        {
            return db.transacts;
        }

        // GET: odata/transacts(5)
        [EnableQuery]
        public SingleResult<transact> Gettransact([FromODataUri] int key)
        {
            return SingleResult.Create(db.transacts.Where(transact => transact.TransacNumber == key));
        }

        // PUT: odata/transacts(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<transact> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            transact transact = db.transacts.Find(key);
            if (transact == null)
            {
                return NotFound();
            }

            patch.Put(transact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!transactExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(transact);
        }

        // POST: odata/transacts
        public IHttpActionResult Post(transact transact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.transacts.Add(transact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (transactExists(transact.TransacNumber))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(transact);
        }

        // PATCH: odata/transacts(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<transact> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            transact transact = db.transacts.Find(key);
            if (transact == null)
            {
                return NotFound();
            }

            patch.Patch(transact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!transactExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(transact);
        }

        // DELETE: odata/transacts(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            transact transact = db.transacts.Find(key);
            if (transact == null)
            {
                return NotFound();
            }

            db.transacts.Remove(transact);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool transactExists(int key)
        {
            return db.transacts.Count(e => e.TransacNumber == key) > 0;
        }
    }
}
