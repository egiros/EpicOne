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
    public class DettagliOrdineController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: DettagliOrdine
        public ActionResult Index(int? ordineID)
        {
            if (!ordineID.HasValue)
            {
                // Gestisci il caso in cui ordineID è null
                // Ad esempio, potresti reindirizzare l'utente a una pagina di errore
                return RedirectToAction("Error", "Home");
            }

            var dettagliOrdine = db.DettagliOrdine
                                   .Include(d => d.Biglietti.Eventi)
                                   .Include(d => d.Ordini)
                                   .Where(d => d.OrdineID == ordineID.Value);

            return View(dettagliOrdine.ToList());
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
