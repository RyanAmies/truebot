using System;
using System.ComponentModel.DataAnnotations;

namespace TRUEbot.Bot.Data.Models
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
