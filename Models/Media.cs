using DAL;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wikimedia.Models
{
    public class Media : Record
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string YoutubeId { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; }
        public int UserId {  get; set; }
        public bool Shared { get; set; }

        [JsonIgnore]
        public User Publisher => DB.Users.Get(UserId);
    }
}