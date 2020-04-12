using System;
using System.ComponentModel.DataAnnotations;

namespace TRUEbot.Bot.Data.Models
{
    public class Kill
    {
        [Key]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public DateTime KilledOn { get; set; }

        public int Power { get; set; }

        [Required]
        public string KilledBy { get; set; }

        [Required]
        public string KilledByNormalised { get; set; }
        
        public virtual Player Player { get; set; }
        
        [Required]
        public string ImageLink { get; set; }
    }
}
