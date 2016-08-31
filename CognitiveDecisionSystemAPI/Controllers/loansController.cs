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
    builder.EntitySet<loan>("loans");
    builder.EntitySet<bank>("banks"); 
    builder.EntitySet<supplier>("suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class loansController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/loans
        [EnableQuery]
        public IQueryable<loan> Getloans()
        {
            return db.loans;
        }

        // GET: odata/loans(5)
        [EnableQuery]
        public SingleResult<loan> Getloan([FromODataUri] int key)
        {
            return SingleResult.Create(db.loans.Where(loan => loan.LoanNum == key));
        }

        // PUT: odata/loans(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<loan> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            loan loan = db.loans.Find(key);
            if (loan == null)
            {
                return NotFound();
            }

            patch.Put(loan);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!loanExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(loan);
        }

        // POST: odata/loans
        public IHttpActionResult Post(loan loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.loans.Add(loan);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (loanExists(loan.LoanNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(loan);
        }

        // PATCH: odata/loans(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<loan> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            loan loan = db.loans.Find(key);
            if (loan == null)
            {
                return NotFound();
            }

            patch.Patch(loan);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!loanExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(loan);
        }

        // DELETE: odata/loans(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            loan loan = db.loans.Find(key);
            if (loan == null)
            {
                return NotFound();
            }

            db.loans.Remove(loan);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/loans(5)/bank
        [EnableQuery]
        public SingleResult<bank> Getbank([FromODataUri] int key)
        {
            return SingleResult.Create(db.loans.Where(m => m.LoanNum == key).Select(m => m.bank));
        }

        // GET: odata/loans(5)/supplier
        [EnableQuery]
        public SingleResult<supplier> Getsupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.loans.Where(m => m.LoanNum == key).Select(m => m.supplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool loanExists(int key)
        {
            return db.loans.Count(e => e.LoanNum == key) > 0;
        }
    }
}
