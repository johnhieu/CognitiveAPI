//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CognitiveDecisionSystemAPI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    
    public partial class product
    {
        [Key]
        public int ProductId { get; set; }
        public int SupplierID { get; set; }
        public Nullable<int> Stocklevel { get; set; }
        public Nullable<int> UnitPrice { get; set; }
    
        public virtual supplier supplier { get; set; }
    }
}
