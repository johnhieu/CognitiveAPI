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
    
    public partial class transact
    {
        [Key]
        public int TransacNumber { get; set; }
        public int ReceiptNumber { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public Nullable<int> Amount { get; set; }
    }
}
