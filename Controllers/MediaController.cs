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
            if (Session["CurrentSelectedCategory"] == null) Session["CurrentSelectedCategory"] = 0;
            if (Session["Categories"] == null) Session["Categories"] = DB.MediasCategories();
            if (Session["SortByTitle"] == null) Session["SortByTitle"] = true;
            if (Session["SortByDate"] == null) Session["SortByDate"] = true;
            if (Session["SortAscending"] == null) Session["SortAscending"] = false;
        }

        private void ResetCurrentMediaInfo()
        {
            Session["CurrentMediaId"] = 0;
            Session["CurrentMediaTitle"] = "";
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
        [Authorize(Roles = "Writer")]
        public ActionResult Create()
        {
            return View(new Media());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Writer")]
        public ActionResult Create(Media media)
        {
            DB.Medias.Add(media);
            return RedirectToAction("List");
        }
        [Authorize(Roles = "Writer")]
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
        [Authorize(Roles = "Writer")]
        public ActionResult Edit(Media media)
        {
            int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;

            Media storedMedia = DB.Medias.Get(id);
            if (storedMedia != null)
            {
                media.Id = id;
                media.LastModified = DateTime.Now;
                DB.Medias.Update(media);
            }
            return RedirectToAction("Details/" + id);
        }
        [Authorize(Roles = "Writer")]
        public ActionResult Delete()
        {
            int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;
            if (id != 0)
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
    }
}