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
        public int UserId { get; set; } 
        public bool Shared { get; set; } = true;
        public List<Like> Likes
        {
            get { return DB.Likes.ToList().Where(l => l.MediaId == Id).ToList(); }
        }
        public bool LikedByConnectedUser
        {
            get
            {
                var user = User.ConnectedUser;
                if (user == null) return false;
                return DB.Likes.ToList().Any(l => l.MediaId == Id && l.UserId == user.Id);
            }
        }
        [JsonIgnore]
        public User Publisher => DB.Users.Get(UserId).Copy();
    }
}