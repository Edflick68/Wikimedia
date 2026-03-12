using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wikimedia.Models
{
    public class Media
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Category { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; } 
        public string YoutubeId { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;
    }
}