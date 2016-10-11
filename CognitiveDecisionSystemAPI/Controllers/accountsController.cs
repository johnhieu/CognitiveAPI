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
using System.Web.Http.Cors;

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

    // By putting EnableCors at the top of the class, this will enable the cross-domain request to be accepted 
    // from other domains since some browsers (Google Chrome,...) have a restricted same-origin policy.
    // So putting EnableCors for the controllers that you want to allow cross-domain request to be accepted.
    // In this case, we allow the domain: http://loocalhost:52527 which is the main 
    // domain for our system to use. We can also add other domains as well if we need to distribute the API to other
    // servers.
    [EnableCors(origins: "http://localhost:52527", headers: "*", methods: "*")]
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
            // Prepare query to retreive data
            List<string> attributeName = new List<string>();
            MySqlConnection conn = new MySqlConnection("server=localhost; port=3306;database=" + database + ";user=root;password=lovingyou;");
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT COLUMN_NAME, DATA_TYPE, COLUMN_KEY FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='"+database+"'  AND `TABLE_NAME`='"+table+"';");
            cmd.Connection = conn;
           
            MySqlDataReader reader = cmd.ExecuteReader();

            // Read data in the form of: "Attribute" "Attribute_Type" "Key(if having any)"
            while(reader.Read())
            {
                string data = reader.GetString(0) + " " + reader.GetString(1);
                if(reader.GetString(2).Length != 0)
                {
                    data += " " + reader.GetString(2);
                }
                attributeName.Add(data);               
            }
            conn.Close();

            return Ok(attributeName);

        }

        // Extra functions to return a list of tables
        [HttpGet]
        [ODataRoute("GetTables(Database={database})")]
        public IHttpActionResult GetTables([FromODataUri] string database)
        {
            // Prepare the query 
            List<string> tableName = new List<string>();
            MySqlConnection conn = new MySqlConnection("server=localhost; port=3306;database=" + database + ";user=root;password=lovingyou;");
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA='" + database + "' ");
            cmd.Connection = conn;

            MySqlDataReader reader = cmd.ExecuteReader();
            // Read data and store in tableName List
                while (reader.Read())
                {
                    tableName.Add(reader.GetString(0));

                }

            conn.Close();
            return Ok(tableName);

        }
       
        // Extra functions to get the referenced tables for a particular table
        [HttpGet]
        [ODataRoute("GetReferencedTables(Database={database},Table={table})")]
        public IHttpActionResult GetReferencedTables([FromODataUri] string database, [FromODataUri] string table)
        {
            // Prepare the query
            List<string> referencedTables = new List<string>();
            MySqlConnection conn = new MySqlConnection("server=localhost; port=3306;database=" + database + ";user=root;password=lovingyou;");
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT REFERENCED_TABLE_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" + database + "'  AND `TABLE_NAME`='" + table + "'  AND REFERENCED_TABLE_NAME IS NOT NULL;");
            cmd.Connection = conn;

            // Read data and store in referencedTables List
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                referencedTables.Add(reader.GetString(0));
            }
            conn.Close();

            return Ok(referencedTables);
        }

        // Extra functions to get the referenced tables for a particular table
        [HttpGet]
        [ODataRoute("GetTablesReferencingTable(Database={database},Table={table})")]
        public IHttpActionResult GetTablesReferencingTable([FromODataUri] string database, [FromODataUri] string table)
        {
            List<string> referencedTables = new List<string>();
            List<string> temp = new List<string>();

            MySqlConnection conn = new MySqlConnection("server=localhost; port=3306;database=" + database + ";user=root;password=lovingyou;");
            conn.Open();
            
            // This query is used to retreive all the table that references the chosen table
            MySqlCommand cmd = new MySqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" + database + "'  AND `REFERENCED_TABLE_NAME`='" + table + "'  AND REFERENCED_TABLE_NAME IS NOT NULL;");
            cmd.Connection = conn;

           using( MySqlDataReader reader = cmd.ExecuteReader())
            {
                  while (reader.Read())
                  {
                        temp.Add(reader.GetString(0));              
                  }
        
            }
          

            for (int i = 0; i < temp.Count; i++ )
            {
                // Check if the table referencing the main table is a result of many-to-many relationship
                String tableName = temp[i];
                bool normalTable = true;
                // MySqlCommand cmd2 = new MySqlCommand("SELECT COLUMN_NAME, COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '" + database + "' AND TABLE_NAME = '" + tableName + "' GROUP BY COLUMN_NAME;");
                // The original query is with COLUMN_NAME to show the keys of that column. The result will return the table with number of time it appears using COUNT since if it appears twice, we know that the column is a primary key and a foregin key as well.
                // So to trim it down, it skip the column name and just select the count only.
                MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '" + database + "' AND TABLE_NAME = '" + tableName + "' GROUP BY COLUMN_NAME;");
                cmd2.Connection = conn;
                using (MySqlDataReader reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(reader.GetInt32(0) == 1)
                        {
                            normalTable = false;
                        }
                    }
                }
               

                // Check if the table is a join table from many to many relationship then add another text into it for example "accounts link supplier", else just add the name of it
                if (normalTable)
                {
                    String linkTo = "";
                    cmd2.CommandText = "SELECT REFERENCED_TABLE_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" + database + "'  AND `TABLE_NAME`='" + tableName + "'  AND REFERENCED_TABLE_NAME <> '" + table + "';";
                    using(MySqlDataReader reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            linkTo += " " + reader.GetString(0);
                        }
                        referencedTables.Add(temp[i] + " link" + linkTo);
                    }
                    
                }
                else
                {
                    referencedTables.Add(temp[i]);
                }
            }

                conn.Close();

            return Ok(referencedTables);
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
