using System.Collections.Generic;

namespace Videos.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsAdultContent { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
    }
}