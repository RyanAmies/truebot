using System;
using System.ComponentModel.DataAnnotations;

namespace TRUEbot.Bot.Data.Models
{
    public class SystemLog
    {
        [Key]
        public int Id {get; set; }

        public int PlayerId {get; set; }
        public virtual Player Player { get; set; }

        public int? SystemId {get; set; }
        public virtual System System { get; set; }

        public DateTime DateUpdated {get; set; }
    }
}
