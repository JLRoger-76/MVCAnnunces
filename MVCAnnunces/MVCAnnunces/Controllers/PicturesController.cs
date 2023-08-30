using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCAnnunces.dal;
using MVCAnnunces.dal.Entity;
using MVCAnnunces.Repository;

namespace MVCAnnunces.Controllers
{
    public class PicturesController : Controller
    {
        private readonly GenericRepository<Picture> repository = null;
        public PicturesController()
        {
            repository = new GenericRepository<Picture>();
        }

        // GET: Pictures
        public ActionResult Index()
        {
            if (Session["UserConnected"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pictures = repository.GetAll();
            return View(pictures);
        }

        // GET: Pictures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null | (Session["UserConnected"] == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = repository.GetById(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        
        // GET: Pictures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null | (Session["UserConnected"] == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = repository.GetById(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Picture picture = repository.GetById(id);
            repository.Delete(id);
            repository.Save();

            string fullPath = Request.MapPath(picture.Filename);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return RedirectToAction("../Annunces/ByUser");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
