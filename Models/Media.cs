using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wikimedia.Models;

namespace Models
{
    public enum MediaSortBy { Title, PublishDate, Likes }

    public class Media : Record
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string YoutubeId { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;

        public int OwnerId { get; set; } = 1;
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
        public User Owner => DB.Users.Get(OwnerId).Copy();

        public override bool IsValid()
        {
            if (!HasRequiredLength(Title, 1)) return false;
            if (!HasRequiredLength(Category, 1)) return false;
            if (!HasRequiredLength(Description, 1)) return false;
            if (DB.Medias.ToList().Where(m => m.YoutubeId == YoutubeId && m.Id != Id).Any()) return false;
            return true;
        }
    }
}