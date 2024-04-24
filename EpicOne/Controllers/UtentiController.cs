using EpicOne.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EpicOne.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtentiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Utenti
        public ActionResult Index()
        {
            return View(db.Utenti.ToList());
        }
        public ActionResult IsAdmin(int id)
        {
            //change the user to admin
            Utenti utente = db.Utenti.Find(id);
            utente.IsAdmin = !utente.IsAdmin;
            db.Entry(utente).State = EntityState.Modified;
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


        public ActionResult Delete(int id)
        {
            Utenti utente = db.Utenti.Find(id);
            db.Utenti.Remove(utente);
            db.SaveChanges();
            TempData["Message"] = "Utente eliminato";
            return RedirectToAction("Index");
        }
    }
}