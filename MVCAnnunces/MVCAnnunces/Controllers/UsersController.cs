using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MVCAnnunces.dal;
using MVCAnnunces.dal.Entity;


namespace MVCAnnunces.Controllers
{
    public class UsersController : Controller
    {
        private MyEntities db = new MyEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Connect
        public ActionResult Connect([Bind(Include = "UserId,Pseudo,Password,Email")] User user)
        {
        using (SHA256 sha256Hash = SHA256.Create())
                if (ModelState.IsValid)
            {
                //hash the password in sha256
                string HashPassword = BitConverter.ToString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password))).Replace("-", String.Empty);
                User userConnected = db.Users.FirstOrDefault(u => u.Password == HashPassword);
                if (userConnected != null)
                {
                    this.Session["UserConnected"] = userConnected;
                        if (userConnected.Role == "Admin")
                        {
                            return RedirectToAction("../Categories");
                        }
                        else
                        {
                            return RedirectToAction("../Annunces/ByUser");
                        }      
                }
            }
            return View(user);
        }
        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null | (Session["UserConnected"] == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Init
        public ActionResult Init()
        {
            return View();
        }

        // POST: Users/Init
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Init([Bind(Include = "UserId,Pseudo,Password,Email")] User user)
        {
            User userModified = db.Users.SingleOrDefault(a => a.Pseudo == user.Pseudo & a.Email==user.Email);
            using (SHA256 sha256Hash = SHA256.Create())
                if (userModified!=null)
                {
                    // Create a SHA256  
                    // ComputeHash - returns byte array 
                    userModified.Password = BitConverter.ToString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password))).Replace("-", String.Empty);
                    db.Entry(userModified).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Connect");
                }
            return View(user);
        }


        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Pseudo,Password,Email,PostalCode")] User user)
        {
            using (SHA256 sha256Hash = SHA256.Create()) 
            if (ModelState.IsValid)
            {
                    // Create a SHA256  
                    // ComputeHash - returns byte array 
                    user.Password = BitConverter.ToString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password))).Replace("-", String.Empty);
                    db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null | (Session["UserConnected"] == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Pseudo,Password,Email,Role,PostalCode")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null | (Session["UserConnected"] == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
