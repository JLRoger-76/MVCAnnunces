
using System.Net;
using System.Web.Mvc;
using MVCAnnunces.dal.Entity;
using MVCAnnunces.Repository;

namespace MVCAnnunces.Controllers
{
    public class CategoriesController : Controller
    {
        private IRepository<Category> repository = null;
        public CategoriesController()
        {
            this.repository = new GenericRepository<Category>();
        }
        public CategoriesController(IRepository<Category> repository)
        {
            this.repository = repository;
        }
        public bool isNotAdmin()
        {
            var userConncected = Session["UserConnected"] as User;
            return !(userConncected != null && userConncected.Role != null);              
        }

        // GET: Categories
        public ActionResult Index()
        {
            if (isNotAdmin())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);              
            }
            return View(repository.GetAll());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {          
            if (isNotAdmin() | id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(repository.GetById(id));
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            if (isNotAdmin())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["Categories"] = repository.GetAll(); // Send this list to the view
            return View();
        }

        // POST: Categories/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                repository.Insert(model);
                repository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (isNotAdmin() | id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category model = repository.GetById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewData["Categories"] = repository.GetAll(); // Send this list to the view
            return View(model);
        }

        // POST: Categories/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                repository.Update(model);
                repository.Save();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (isNotAdmin() | id == null)
                {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category model = repository.GetById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            repository.Delete(id);
            repository.Save();
            return RedirectToAction("Index");
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
