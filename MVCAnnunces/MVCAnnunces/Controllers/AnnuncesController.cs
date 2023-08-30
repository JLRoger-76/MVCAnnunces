using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCAnnunces.dal;
using MVCAnnunces.dal.Entity;
using MVCAnnunces.Repository;

namespace MVCAnnunces.Controllers
{
    public class AnnuncesController : Controller
    {
        private readonly GenericRepository<Annunce> annunce_repository = null;
        private readonly GenericRepository<Picture> picture_repository = null;
        private readonly IAnnunceRepository specific_annunce_repository = null;
        private readonly IRepository<Category> category_repository = null;
        public AnnuncesController()
        {
            specific_annunce_repository = new AnnunceRepository();
            annunce_repository = new GenericRepository<Annunce>();
            picture_repository = new GenericRepository<Picture>();
            category_repository = new GenericRepository<Category>();
        }


        public bool isNotAdmin()
        {
            var userConncected = Session["UserConnected"] as User;
            return !(userConncected != null && userConncected.Role != null);
        }
        // GET: Annunces
        public ActionResult Index([Bind(Include = "category,department,searchString,take,page,ordered")] AnnParam param)
        {

            if (param.take == 0) { param.take = 1; }
            param.nbPages = specific_annunce_repository.GetValid(param).Count() / param.take;
            ViewBag.parameters = param;
            ViewData["Categories"] = category_repository.GetAll(); // Send this list to the view
            var model = specific_annunce_repository.GetValidPaged(param);
            return View(model);
        }

        // POST: Annunces
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "category,department,searchString,take,page,ordered")] AnnParam param, int? t)
        {
            param.nbPages = specific_annunce_repository.GetValid(param).Count() / param.take;
            ViewBag.parameters = param;
            ViewData["Categories"] = category_repository.GetAll(); // Send this list to the view
            var model = specific_annunce_repository.GetValidPaged(param);
            return View(model);
        }

        // GET: Annunces/DeletePerime
        public ActionResult DeletePerime()
        {
            if (isNotAdmin())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = specific_annunce_repository.GetPerimed();
            return View(model);
        }

        // GET: Annunces/ByCategory/5
        public ActionResult ByCategory(int? id)
        {
            var model = specific_annunce_repository.GetByCategory(id);
            return View(model);
        }

        // GET: Annunces/ByUser
        public ActionResult ByUser()
        {
            var userConncected = Session["UserConnected"] as User;
            
            if (userConncected == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var id = userConncected.UserId;
            var model = specific_annunce_repository.GetByUser(id);
            return View(model);
        }

        // GET: Annunces/Details/5
        public ActionResult Details(int? id, [Bind(Include = "category,department,searchString,take,page,ordered")] AnnParam param)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annunce annunce = specific_annunce_repository.GetById(id);
            if (annunce == null)
            {
                return HttpNotFound();
            }
            annunce.Vues += 1;
            annunce_repository.Update(annunce);
            annunce_repository.Save();
            ViewBag.parameters = param;

            return View(annunce);
        }

            // GET: Annunces/Create
            public ActionResult Create()
        {
            var userConncected = Session["UserConnected"] as User;
            if (userConncected == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["Categories"] = category_repository.GetAll(); // Send this list to the view
            return View();
        }

        // POST: Annunces/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnnunceId,Title,Detail,Posted,Limit,CategoryId")] Annunce annunce, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                annunce.Vues = 0;
                var userConncected = Session["UserConnected"] as User;
                annunce.UserId = userConncected.UserId;
                annunce_repository.Insert(annunce);
                annunce_repository.Save();
                foreach (HttpPostedFileBase file in files)
                {
                    string relativePath = "~/UploadedPictures/" + Path.GetFileName(file.FileName);
                    Picture model = new Picture { Filename = relativePath, AnnunceId = annunce.AnnunceId };
                    picture_repository.Insert(model);
                    picture_repository.Save();
                }
                SavePictures(annunce.AnnunceId, files);
                return RedirectToAction("ByUser");

            }
            return View(annunce);
        }

        // GET: Annunces/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null | (Session["UserConnected"] == null ))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = specific_annunce_repository.GetById(id);           
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewData["Categories"] = category_repository.GetAll(); // Send this list to the view
            return View(model);
        }

        // POST: Annunces/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AnnunceId,Title,Detail,Posted,Limit,CategoryId")] Annunce annunce,List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var userConncected = Session["UserConnected"] as User;
                annunce.UserId = userConncected.UserId;
                annunce_repository.Update(annunce);
                annunce_repository.Save();
                try
                {
                    if (files!=null)
                    {
                        foreach (HttpPostedFileBase file in files)
                        {
                            if (file != null)
                            {
                                string relativePath = "~/UploadedPictures/" + Path.GetFileName(file.FileName);
                                Picture model = new Picture { Filename = relativePath, AnnunceId = annunce.AnnunceId };
                                picture_repository.Insert(model);
                                picture_repository.Save();
                            }
                        }
                        SavePictures(annunce.AnnunceId, files);
                    }
                    ViewBag.Message = "File Uploaded Successfully!!";
                }
                catch
                {
                    ViewBag.Message = "File upload failed!!";
                }
                return RedirectToAction("ByUser");
            }
            return View(annunce);
        }

        // GET: Annunces/Delete/5
        public ActionResult Delete(int? id, bool ?active)
        {
            if (id == null | (Session["UserConnected"] == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annunce annunce = annunce_repository.GetById(id);
            if (annunce == null)
            {
                return HttpNotFound();
            }
            return View(annunce);
        }

        // POST: Annunces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, bool active)
        {
            Annunce annunce = specific_annunce_repository.GetById(id);
            annunce_repository.Delete(id);

            var pictures = annunce.Pictures.ToList();
            foreach (Picture picture in pictures)
            {
                string fullPath = Request.MapPath(picture.Filename);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            annunce_repository.Save();
            if (active)
            {
                return RedirectToAction("ByUser");
            }
            else
            {
                return RedirectToAction("DeletePerime");
            }
           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                annunce_repository.Dispose();
            }
            base.Dispose(disposing);
        }
        void SavePictures(int annunceId, List<HttpPostedFileBase> files)
        {
            try
            {
                if (files != null)
                {
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            string relativePath = "~/UploadedPictures/" + Path.GetFileName(file.FileName);
                            string physicalPath = Server.MapPath(relativePath);
                            file.SaveAs(physicalPath);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
