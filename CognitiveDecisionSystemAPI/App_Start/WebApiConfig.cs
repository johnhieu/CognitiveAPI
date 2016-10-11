    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using CognitiveDecisionSystemAPI.Models;


namespace CognitiveDecisionSystemAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API Cors
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<account>("accounts");
            builder.EntitySet<customer>("customers");
            builder.EntitySet<employee>("employees");
            builder.EntitySet<supplier>("suppliers");
            builder.EntitySet<bank>("banks");
            builder.EntitySet<loan>("loans");
            builder.EntitySet<payment>("payments");
            builder.EntitySet<product>("products");
            builder.EntitySet<sale>("sales");
            builder.EntitySet<transact>("transacts");

            // Configure the OData path for custom functions

            // For only 1 parameter, we can omit the configuration into one line for this GetTables functions
            builder.Function("GetTables").Returns<List<string>>().Parameter<string>("Database");
            
            // Configuration for GetAttributes functions with two parameters : Database and table
            var getAttributeFunction = builder.Function("GetAttributes");
            getAttributeFunction.Parameter<string>("Database");
            getAttributeFunction.Parameter<string>("Table");
            getAttributeFunction.Returns<List<string>>();

            // Configuration for GetReferencedTables functions with two parameters : Database and table
            var getReferencedTablesFunction = builder.Function("GetReferencedTables");
            getReferencedTablesFunction.Parameter<string>("Database");
            getReferencedTablesFunction.Parameter<string>("Table");
            getReferencedTablesFunction.Returns<List<string>>();

            // Configuration for GetTablesReferencingTable functions with two parameters : Database and table
            var getTablesReferencingTableFunction = builder.Function("GetTablesReferencingTable");
            getTablesReferencingTableFunction.Parameter<string>("Database");
            getTablesReferencingTableFunction.Parameter<string>("Table");
            getTablesReferencingTableFunction.Returns<List<string>>();

            config.MapODataServiceRoute("odata", "Service", builder.GetEdmModel());

           
        }


    }
}
