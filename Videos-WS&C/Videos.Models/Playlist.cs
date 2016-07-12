using System.Collections.Generic;

namespace Videos.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Playlist
    {
        private ICollection<Tag> tags;
        private ICollection<Video> videos;

        public Playlist()
        {
            this.tags = new HashSet<Tag>();
            this.videos = new HashSet<Video>();
        }
        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsAdultContent { get; set; }

        public string OwnerId { get; set; }

        public virtual Location Location { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<Tag> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

        public virtual ICollection<Video> Videos
        {
            get { return this.videos; }
            set { this.videos = value; }
        }
    }
}