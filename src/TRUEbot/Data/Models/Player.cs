using System;
using System.ComponentModel.DataAnnotations;

namespace TRUEbot.Data.Models
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

        public string Location { get; set; }

        public string NormalizedLocation { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
