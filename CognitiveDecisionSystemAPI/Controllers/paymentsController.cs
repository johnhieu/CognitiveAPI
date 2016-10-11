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
    builder.EntitySet<payment>("payments");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    // Please look at the comments in the accountsController for more details
    [EnableCors(origins: "http://localhost:52527", headers: "*", methods: "*")]
    public class paymentsController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/payments
        [EnableQuery]
        public IQueryable<payment> Getpayments()
        {
            return db.payments;
        }

        // GET: odata/payments(5)
        [EnableQuery]
        public SingleResult<payment> Getpayment([FromODataUri] int key)
        {
            return SingleResult.Create(db.payments.Where(payment => payment.PaymentNum == key));
        }

        // PUT: odata/payments(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<payment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            payment payment = db.payments.Find(key);
            if (payment == null)
            {
                return NotFound();
            }

            patch.Put(payment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!paymentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(payment);
        }

        // POST: odata/payments
        public IHttpActionResult Post(payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.payments.Add(payment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (paymentExists(payment.PaymentNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(payment);
        }

        // PATCH: odata/payments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<payment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            payment payment = db.payments.Find(key);
            if (payment == null)
            {
                return NotFound();
            }

            patch.Patch(payment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!paymentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(payment);
        }

        // DELETE: odata/payments(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            payment payment = db.payments.Find(key);
            if (payment == null)
            {
                return NotFound();
            }

            db.payments.Remove(payment);
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

        private bool paymentExists(int key)
        {
            return db.payments.Count(e => e.PaymentNum == key) > 0;
        }
    }
}
