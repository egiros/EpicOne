using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EpicOne.Models;

namespace EpicOne.Controllers
{
    [Authorize]
    public class EventiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Eventi
        public ActionResult Index(int? categoriaId, int? luogoId, string orderBy, int? limit)
        {
            var eventi = db.Eventi.AsQueryable();

            if (categoriaId.HasValue)
            {
                var categoria = db.Categorie.Find(categoriaId.Value);
                if (categoria != null)
                {
                    TempData["NomeCategoria"] = categoria.Nome;
                }
                eventi = eventi.Where(e => e.Categorie.CategoriaID == categoriaId);
            }

            if (luogoId.HasValue)
            {
                eventi = eventi.Where(e => e.Luoghi.LuogoID == luogoId);
            }

            // Ordina gli eventi
            switch (orderBy)
            {
                case "Titolo":
                    eventi = eventi.OrderBy(e => e.Titolo);
                    break;
                case "DataCrescente":
                    eventi = eventi.OrderBy(e => e.DataOra);
                    break;
                case "DataDecrescente":
                default:
                    eventi = eventi.OrderByDescending(e => e.DataOra);
                    break;
            }

            // Limita gli eventi
            if (limit.HasValue && limit.Value > 0)
            {
                eventi = eventi.Take(limit.Value);
            }
            else if (limit.HasValue && limit.Value == -1)
            {
                // Non applicare alcun limite
            }
            else
            {
                // Se non è specificato un limite, limita a 100
                eventi = eventi.Take(100);
            }

            var eventiList = eventi.ToList();

            if (Request.IsAjaxRequest())
            {
                var data = eventiList.Select(e => new
                {
                    EventoID = e.EventoID,
                    Titolo = e.Titolo,
                    LuogoNome = e.Luoghi.Nome,
                    LuogoCitta = e.Luoghi.Città,
                    DataOra = e.DataOra.ToString("dddd dd/MM/yyyy HH:mm"),
                    Immagine = e.Immagine
                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return View(eventi.ToList());
        }



        // GET: Eventi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventi eventi = db.Eventi.Find(id);
            if (eventi == null)
            {
                return HttpNotFound();
            }
            return View(eventi);
        }

        // GET: Eventi/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoriaID = new SelectList(db.Categorie, "CategoriaID", "Nome");
            ViewBag.LuogoID = new SelectList(db.Luoghi, "LuogoID", "Nome");
            ViewBag.OrganizzatoreID = new SelectList(db.Utenti, "UserID", "Nome");
            return View();
        }
        // Action Create che permette l'upload di immagine
        // e la creazione di un nuovo evento
        // con debug per verificare eventuali errori

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventoID,Titolo,Descrizione,DataOra,LuogoID,CategoriaID,OrganizzatoreID,PrezzoBase,QuantitàTotaleBiglietti,BigliettiRimanenti,Immagine")] Eventi eventi, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Img"), fileName);
                    if (!Directory.Exists(Server.MapPath("~/Content/Img")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Img"));
                    }
                    file.SaveAs(path);
                    eventi.Immagine = "/Content/Img/" + fileName;
                }
                else
                {
                    eventi.Immagine = "/Content/Img/Default.png";
                }

                if (ModelState.IsValid)
                {
                    db.Eventi.Add(eventi);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Errore durante il salvataggio del file: " + ex.Message);
            }
            return View(eventi);
        }



        // GET: Eventi/Edit/5

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventi eventi = db.Eventi.Find(id);
            if (eventi == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoriaID = new SelectList(db.Categorie, "CategoriaID", "Nome", eventi.CategoriaID);
            ViewBag.LuogoID = new SelectList(db.Luoghi, "LuogoID", "Nome", eventi.LuogoID);
            ViewBag.OrganizzatoreID = new SelectList(db.Utenti, "UserID", "Nome", eventi.OrganizzatoreID);
            return View(eventi);
        }

        // POST: Eventi/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.

        //Action per la modifica di un evento
        // Permette di cambiare l'immagine del evento
        //Ma in caso non venga caricata nessuna immagine, verra' comunque salvata la precedente



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "EventoID,Titolo,Descrizione,DataOra,LuogoID,CategoriaID,OrganizzatoreID,PrezzoBase,QuantitàTotaleBiglietti,BigliettiRimanenti,Immagine")] Eventi eventi, HttpPostedFileBase File)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var oldEvent = db.Eventi.AsNoTracking().FirstOrDefault(e => e.EventoID == id);
                    if (File != null && File.ContentLength > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(oldEvent.Immagine))
                        {
                            var existingImagePath = Path.Combine(Server.MapPath("~/Content/Img/"), oldEvent.Immagine);
                            if (System.IO.File.Exists(existingImagePath))
                            {
                                System.IO.File.Delete(existingImagePath);
                            }
                        }

                        var fileName = Path.GetFileNameWithoutExtension(File.FileName) + DateTime.Now.Ticks + Path.GetExtension(File.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Img/"), fileName);
                        File.SaveAs(path);

                        eventi.Immagine = "/Content/Img/" + fileName;
                    }
                    else
                    {
                        eventi.Immagine = oldEvent.Immagine;
                    }

                    db.Entry(eventi).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["CreateMess"] = "Evento modificato correttamente";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return View(eventi);
        }


        // GET: Eventi/Delete/5

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventi eventi = db.Eventi.Find(id);
            if (eventi == null)
            {
                return HttpNotFound();
            }
            return View(eventi);
        }

        // POST: Eventi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eventi eventi = db.Eventi.Find(id);
            db.Eventi.Remove(eventi);
            db.SaveChanges();
            TempData["CreateMess"] = "Evento eliminato correttamente";
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


        //Action per aggiungere un Evento al carrello
        //Crea una lista di eventi e la aggiunge alla sessione

        public ActionResult AddToCart(int id, int quantita)
        {
            var eventi = db.Eventi.Find(id);
            if (eventi != null)
            {
                var cart = Session["cart"] as List<CartItem> ?? new List<CartItem>();
                var item = cart.FirstOrDefault(p => p.Evento.EventoID == id);
                if (item != null)
                {
                    item.QuantitaBiglietti += quantita;
                }
                else
                {
                    cart.Add(new CartItem { Evento = eventi, QuantitaBiglietti = quantita });
                }
                Session["cart"] = cart;
                TempData["CreateMess"] = "Evento aggiunto al carrello";
            }
            return RedirectToAction("Index");
        }


    }
}
