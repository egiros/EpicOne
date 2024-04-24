using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EpicOne.Models;

namespace EpicOne.Controllers
{
    public class OrdiniController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Ordini
        public ActionResult Index()
        {
            var username = User.Identity.Name; // Ottieni il nome dell'utente corrente
            //return Content("Username: " + username);
            var user = db.Utenti.SingleOrDefault(u => u.Email == username); // Trova l'utente nel database

            if (user == null)
            {
                // Reindirizza l'utente a una pagina di errore
                return RedirectToAction("Error", "Home");
            }

            var ordini = db.Ordini.Where(o => o.UserID == user.UserID); // Filtra gli ordini per l'utente corrente

            if (!ordini.Any()) // Se l'utente non ha effettuato ordini, mostra la vista Empty
            {
                return View("Empty");
            }

            if (User.IsInRole("Admin")) // Se l'utente è un amministratore, mostra tutti gli ordini
            {
                ordini = db.Ordini;
            }

            return View(ordini.ToList());
        }

        // GET: Ordini/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // GET: Ordini/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Utenti, "UserID", "Nome");
            return View();
        }

        // POST: Ordini/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrdineID,UserID,DataOraOrdine,Totale,Stato")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Ordini.Add(ordini);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Utenti, "UserID", "Nome", ordini.UserID);
            return View(ordini);
        }

        // GET: Ordini/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Utenti, "UserID", "Nome", ordini.UserID);
            return View(ordini);
        }

        // POST: Ordini/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrdineID,UserID,DataOraOrdine,Totale,Stato")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordini).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Utenti, "UserID", "Nome", ordini.UserID);
            return View(ordini);
        }

        // GET: Ordini/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordini ordini = db.Ordini.Include(o => o.DettagliOrdine).SingleOrDefault(o => o.OrdineID == id);
            if (ordini != null)
            {
                // Rimuovi prima i dettagli dell'ordine correlati
                db.DettagliOrdine.RemoveRange(ordini.DettagliOrdine);

                // Poi rimuovi l'ordine
                db.Ordini.Remove(ordini);
                db.SaveChanges();
            }
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


        // Gestione dell'evasione degli ordini
        [Authorize(Roles = "Admin")]
        public ActionResult ToggleEvaso(int id)
        {
            ModelDbContext db = new ModelDbContext();
            var ordine = db.Ordini.Find(id);
            if (ordine != null)
            {
                ordine.isEvaso = !ordine.isEvaso;
                db.Entry(ordine).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
