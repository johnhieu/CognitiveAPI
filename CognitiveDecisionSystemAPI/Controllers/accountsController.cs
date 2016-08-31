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
using MySql.Data.MySqlClient;

namespace CognitiveDecisionSystemAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using CognitiveDecisionSystemAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<account>("accounts");
    builder.EntitySet<supplier>("suppliers"); 
    builder.EntitySet<customer>("customers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class accountsController : ODataController
    {
        private financeEntities1 db = new financeEntities1();

        // GET: odata/accounts
        [EnableQuery]
        public IQueryable<account> Getaccounts()
        {
            return db.accounts;
        }

        // GET: odata/accounts(5)
        [EnableQuery]
        public SingleResult<account> Getaccount([FromODataUri] int key)
        {
            return SingleResult.Create(db.accounts.Where(account => account.ReceiptNumber == key));
        }

        // PUT: odata/accounts(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<account> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            account account = db.accounts.Find(key);
            if (account == null)
            {
                return NotFound();
            }

            patch.Put(account);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!accountExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(account);
        }

        // POST: odata/accounts
        public IHttpActionResult Post(account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.accounts.Add(account);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (accountExists(account.ReceiptNumber))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(account);
        }

        // PATCH: odata/accounts(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<account> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            account account = db.accounts.Find(key);
            if (account == null)
            {
                return NotFound();
            }

            patch.Patch(account);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!accountExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(account);
        }

        // DELETE: odata/accounts(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            account account = db.accounts.Find(key);
            if (account == null)
            {
                return NotFound();
            }

            db.accounts.Remove(account);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/accounts(5)/supplier
        [EnableQuery]
        public SingleResult<supplier> Getsupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.accounts.Where(m => m.ReceiptNumber == key).Select(m => m.supplier));
        }

        // GET: odata/accounts(5)/customer
        [EnableQuery]
        public SingleResult<customer> Getcustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.accounts.Where(m => m.ReceiptNumber == key).Select(m => m.customer));
        }

        // Functions to return a list of attributes
        [HttpGet]
        [ODataRoute("GetAttributes(Database={database},Table={table})")]
        public IHttpActionResult GetAttributes([FromODataUri] string database, [FromODataUri] string table)
        {
            List<string> attributeName = new List<string>();
            MySqlConnection conn = new MySqlConnection("server=localhost; port=3306;database=" + database + ";user=root;password=lovingyou;");
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='"+database+"'  AND `TABLE_NAME`='"+table+"';");
            cmd.Connection = conn;

            MySqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                attributeName.Add(reader.GetString(0));               
            }
            conn.Close();

            return Ok(attributeName);

        }

        // Extra functions to return a list of tables
        [HttpGet]
        [ODataRoute("GetTables(Database={database})")]
        public IHttpActionResult GetTables([FromODataUri] string database)
        {
            List<string> tableName = new List<string>();
            MySqlConnection conn = new MySqlConnection("server=localhost; port=3306;database=" + database + ";user=root;password=lovingyou;");
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA='" + database + "' ");
            cmd.Connection = conn;

            MySqlDataReader reader = cmd.ExecuteReader();
            
                while (reader.Read())
                {
                    tableName.Add(reader.GetString(0));

                }

            conn.Close();
            return Ok(tableName);

        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool accountExists(int key)
        {
            return db.accounts.Count(e => e.ReceiptNumber == key) > 0;
        }
    }
}
