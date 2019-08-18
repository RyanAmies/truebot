using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TRUEbot.Data.Models
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
