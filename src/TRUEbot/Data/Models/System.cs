using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TRUEbot.Data.Models
{
    public class System
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Faction { get; set; }

        public int Level { get; set; }

        public string NormalizedName { get; set; }

      public virtual ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
