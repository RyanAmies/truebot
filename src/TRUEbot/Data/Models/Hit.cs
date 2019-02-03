using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TRUEbot.Data.Models
{
    public class Hit
    {
        [Key]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public DateTime OrderedOn { get; set; }

        public DateTime? CompletedOn { get; set; }
        
        public string CompletedBy { get; set; }

        [Required]
        public string OrderedBy { get; set; }
        
        public string Reason { get; set; }

        public virtual Player Player { get; set; }
    }
}
