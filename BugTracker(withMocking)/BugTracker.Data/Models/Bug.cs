namespace BugTracker.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Bug
    {
        private ICollection<Comment> comments;

        public Bug()
        {
            this.comments = new HashSet<Comment>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public BugStatus Status { get; set; }

        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public virtual ICollection<Comment> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }
    }
}
