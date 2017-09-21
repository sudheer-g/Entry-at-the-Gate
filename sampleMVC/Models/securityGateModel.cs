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
        [Required(AllowEmptyStrings = false,ErrorMessage ="Required Field")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required Field")]
        public string email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required Field")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string Reason { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EntryTime { get; set; }
        public string EscortID { get; set; }
    }

    public class IdentityProof
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required Field")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string Name { get; set; }
        [Key]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string ContactNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required Field")]
        public string IdProofType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required Field")]
        public string IdProofNumber { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ReportTime { get; set; }
        public string Photo { get; set; }
    }
}