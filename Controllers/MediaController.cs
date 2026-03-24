using DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;
using Models;
using static Controllers.AccessControl;
using Wikimedia.Models;

namespace Controllers
{
    [UserAccess(Access.View)]
    public class MediaController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["CurrentMediaId"] == null) Session["CurrentMediaId"] = 0;
            if (Session["CurrentMediaTitle"] == null) Session["CurrentMediaTitle"] = "";
            if (Session["Search"] == null) Session["Search"] = false;
            if (Session["SearchString"] == null) Session["SearchString"] = "";
            if (Session["CurrentSelectedCategory"] == null) Session["CurrentSelectedCategory"] = "";
            if (Session["Categories"] == null) Session["Categories"] = DB.MediasCategories();
            if (Session["SortByTitle"] == null) Session["SortByTitle"] = true;
            if (Session["SortByDate"] == null) Session["SortByDate"] = true;
            if (Session["SortByLikes"] == null) Session["SortByLikes"] = true;
            if (Session["SortAscending"] == null) Session["SortAscending"] = false;
            ValidateSelectedCategory();
        }

        private void ResetCurrentMediaInfo()
        {
            Session["CurrentMediaId"] = 0;
            Session["CurrentMediaTitle"] = "";
        }
        private void ValidateSelectedCategory()
        {
            if (Session["SelectedCategory"] != null)
            {
                var selectedCategory = (string)Session["SelectedCategory"];
                var Medias = DB.Medias.ToList().Where(c => c.Category == selectedCategory);
                if (Medias.Count() == 0)
                    Session["SelectedCategory"] = "";
            }
        }
        public ActionResult GetMediasCategoriesList(bool forceRefresh = false)
        {
            InitSessionVariables();

            bool search = (bool)Session["Search"];

            if (search)
            {
                return PartialView();
            }
            return null;
        }

        public ActionResult GetMediaDetails(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();
                int mediaId = (int)Session["CurrentMediaId"];
                Media media = DB.Medias.Get(mediaId);
                if(DB.Users.HasChanged || DB.Medias.HasChanged || forceRefresh)
                {
                    return PartialView(media);
                }
                return null;
            }catch(System.Exception ex)
            {
                return Content("Erreur interne" + ex.Message, "text/html");
            }
        }
        public ActionResult GetMedia(bool forceRefresh = false)
        {
            IEnumerable<Media> result = null;
            if (DB.Medias.HasChanged || forceRefresh )
            {
                InitSessionVariables();
                bool search = (bool)Session["Search"];
                string searchString = (string)Session["SearchString"];

                if (search)
                {
                    result = DB.Medias.ToList().Where(c => c.Title.ToLower().Contains(searchString)).OrderBy(c => c.Title);
                    string SelectedCategory = (string)Session["SelectedCategory"];
                    if (SelectedCategory != "")
                        result = result.Where(c => c.Category == SelectedCategory);
                }
                else
                    result = DB.Medias.ToList();
                if ((bool)Session["SortAscending"])
                {
                    if ((bool)Session["SortByTitle"])
                        result = result.OrderBy(c => c.Title);
                    else
                        result = result.OrderBy(c => c.PublishDate);
                }
                else
                {
                    if ((bool)Session["SortByTitle"])
                        result = result.OrderByDescending(c => c.Title);
                    else
                        result = result.OrderByDescending(c => c.PublishDate);
                }
                return PartialView(result);
            }
            return null;
        }

        public ActionResult List()
        {
            ResetCurrentMediaInfo();
            return View();
        }

        public ActionResult ToggleSearch()
        {
            if (Session["Search"] == null) Session["Search"] = false;
            Session["Search"] = !(bool)Session["Search"];
            return RedirectToAction("List");
        }
        public ActionResult ToggleSort()
        {
            Session["SortAscending"] = !(bool)Session["SortAscending"];
            return RedirectToAction("List");
        }
        public ActionResult SortByTitle()
        {
            Session["SortByTitle"] = true;
            return RedirectToAction("List");
        }
        public ActionResult SortByDate()
        {
            Session["SortByDate"] = true;
            Session["SortByTitle"] = false;
            return RedirectToAction("List");
        }
        public ActionResult SortByLikes()
        {
            Session["SortByLikes"] = true;
            Session["SortByTitle"] = false;
            Session["SortByDate"] = false;
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string value)
        {
            Session["SearchString"] = value.ToLower();
            return RedirectToAction("List");
        }

        public ActionResult SetSearchCategory(int value)
        {
            Session["CurrentSelectedCategory"] = value.ToString();
            return RedirectToAction("List");
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Details(int id)
        {
            Session["CurrentMediaId"] = id;
            Media media = DB.Medias.Get(id);
            if (media != null)
            {
                Session["CurrentMediaTitle"] = media.Title;
                return View(media);
            }
            return RedirectToAction("List");
        }
        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View(new Media());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [UserAccess(Access.Write)]

        public ActionResult Create(Media media)
        {
            DB.Medias.Add(media);
            return RedirectToAction("List");
        }
        [UserAccess(Access.Write)]
        public ActionResult Edit()
        {
            int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;
            if (id != 0)
            {
                Media media = DB.Medias.Get(id);
                if (media != null)
                    return View(media);
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [UserAccess(Access.Write)]
        public ActionResult Edit(Media media)
        {
            int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;

            int connectedUser = Models.User.ConnectedUser.Id;
            Media storedMedia = DB.Medias.Get(id);

            if (storedMedia != null && connectedUser == storedMedia.UserId)
            {
                media.Id = id;
                media.LastModified = DateTime.Now;
                DB.Medias.Update(media);
            }
            return RedirectToAction("Details/" + id);
        }
        [UserAccess(Access.Write)]
        public ActionResult Delete()
        {
            int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;

            int connectedUser = Models.User.ConnectedUser.Id;
            Media storedMedia = DB.Medias.Get(id);

            if (id != 0 && connectedUser == storedMedia.UserId)
            {
                DB.Medias.Delete(id);
            }
            return RedirectToAction("List");
        }
        public JsonResult CheckNameConflict(string Name)
        {
            int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;
            return Json(DB.Medias.ToList().Where(c => c.Title == Name && c.Id != id).Any(),
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLastModified(int id)
        {
            Media media = DB.Medias.Get(id);
            if(media == null)
                return Content("");
            return Content(media.LastModified.Ticks.ToString());
        }
        public JsonResult ToggleLike(int id) 
        {
            int userId = Models.User.ConnectedUser.Id;
            Media media = DB.Medias.Get(id);

            if(media == null)
                return Json(new {succes  = false});

            var existing = DB.Likes.ToList().FirstOrDefault(c => c.MediaId == id && c.UserId == userId);
            if(existing == null)
            {
                DB.Likes.Add(new Like() { MediaId = id, UserId = userId });
            }
            else
            {
                DB.Likes.Delete(existing.Id);
            }

            media.LastModified = DateTime.Now;
            DB.Medias.Update(media);

            DB.Medias.HasChanged = true;
            return Json(new { succes = true });
        }
    }
}