using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace sampleMVC.Models
{
    public class Guest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string Reason { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EntryTime { get; set; }
        public string EscortID { get; set; }
    }

    public class IdentityProof
    {
        public string Name { get; set; }
        [Key]
        public string email { get; set; }
        public string ContactNumber { get; set; }
        public string IdProofType { get; set; }
        public string IdProofNumber { get; set; }
        public DateTime ReportTime { get; set; }
        public string Photo { get; set; }
    }
}