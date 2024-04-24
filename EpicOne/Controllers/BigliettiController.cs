using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EpicOne.Models;
using Rotativa;

namespace EpicOne.Controllers
{
    public class BigliettiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Biglietti
        public ActionResult Index(int? id)
        {
            var username = User.Identity.Name; // Ottiene il nome dell'utente corrente
            var user = db.Utenti.SingleOrDefault(u => u.Email == username); // Trova l'utente nel database

            if (user == null)
            {
                // Reindirizza l'utente a una pagina di errore
                return RedirectToAction("Error", "Home");
            }

            var biglietti = db.Biglietti.Where(b => b.UserID == user.UserID); // Filtra i biglietti per l'utente corrente

            if (id.HasValue)
            {
                // Ottieni gli ID dei biglietti per l'ordine specifico
                var bigliettiIds = db.DettagliOrdine.Where(d => d.OrdineID == id.Value).Select(d => d.BigliettoID).ToList();

                // Filtra i biglietti per l'ordine specifico
                biglietti = biglietti.Where(b => bigliettiIds.Contains(b.BigliettoID));
            }

            if (!biglietti.Any()) // Se l'utente non ha acquistato biglietti, mostra la vista Empty
            {
                return View("Empty");
            }

            if (User.IsInRole("Admin")) // Se l'utente è un amministratore, mostra tutti i biglietti generati
            {
                biglietti = db.Biglietti;
            }

            return View(biglietti.ToList());
        }




        // GET: Biglietti/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Biglietti biglietti = db.Biglietti.Find(id);
            if (biglietti == null)
            {
                return HttpNotFound();
            }
            return View(biglietti);
        }

        // POST: Biglietti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Biglietti biglietti = db.Biglietti.Find(id);
            db.Biglietti.Remove(biglietti);
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


        public ActionResult DownloadPDF(int id)
        {
            var biglietto = db.Biglietti.Find(id);
            var report = new ViewAsPdf("Biglietto", biglietto) { FileName = "Biglietto.pdf" };
            return report;
        }

        public ActionResult Biglietto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Biglietti biglietto = db.Biglietti.Find(id);
            if (biglietto == null)
            {
                return HttpNotFound();
            }
            return View(biglietto);
        }

    }

}
