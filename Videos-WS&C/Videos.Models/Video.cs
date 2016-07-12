using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Video
    {
        private ICollection<Tag> tags;

        public Video()
        {
            this.tags = new HashSet<Tag>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public VideoStatus Status { get; set; }

        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<Tag> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

    }
}
