using System;
using System.Collections.Generic;

namespace Videos.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Location
    {
        [Key]
        public int Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }
    }
}