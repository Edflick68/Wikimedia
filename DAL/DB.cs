using Models;
using PhotosManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using Wikimedia.Models;
using EmailHandling;

namespace DAL
{
    public sealed class DB
    {
        #region singleton setup
        private static readonly DB instance = new DB();
        public static DB Instance  { get { return instance; } }
        #endregion

        public static Repository<Media> Medias { get; set; } = new Repository<Media>();
        public static List<string> MediasCategories()
        {
            List<string> Categories = new List<string>();
            foreach (Media media in Medias.ToList().OrderBy(m => m.Category))
            {
                if (Categories.IndexOf(media.Category) == -1)
                {
                    Categories.Add(media.Category);
                }
            }
            return Categories;
        }
        static public UsersRepository Users { get; set; }
            = new UsersRepository();

        static public NotificationsRepository Notifications { get; set; }
            = new NotificationsRepository();

        static public LoginsRepository Logins { get; set; }
            = new LoginsRepository();

        static public EventsRepository Events { get; set; }
            = new EventsRepository();

        static public Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
            = new Repository<UnverifiedEmail>();

        static public Repository<RenewPasswordCommand> RenewPasswordCommands { get; set; }
            = new Repository<RenewPasswordCommand>();

    }
}