using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRUEbot.Bot.Data.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string NormalizedName { get; set; }

        public string Alliance { get; set; }

        public string NormalizedAlliance { get; set; }

        public string Location { get; set; }

        public string NormalizedLocation { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string AddedBy { get; set; }

        public int? Level { get; set; }

        public int? SystemId { get; set; }
        public virtual System System { get; set; }

        public virtual ICollection<SystemLog> SystemLogs { get; set; } = new HashSet<SystemLog>();
        public virtual ICollection<Kill> Kills { get; set; } = new HashSet<Kill>();

    }
}
