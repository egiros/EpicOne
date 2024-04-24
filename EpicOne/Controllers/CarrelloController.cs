using EpicOne.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EpicOne.Controllers
{
    public class CarrelloController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Carrello

        // Mostra il carrello con i prodotti selezionati dall'utente
        // Se il carrello è vuoto, reindirizza alla pagina dei prodotti con un messaggio
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                TempData["CartMessage"] = "Il carrello è vuoto";
                return RedirectToAction("Index", "Eventi");
            }

            return View(cart);
        }




        // Rimuove un prodotto dal carrello
        // Se la quantità del prodotto è maggiore di 1, decrementa la quantità
        public ActionResult Delete(int? id)
        {
            var cart = Session["cart"] as List<CartItem>;

            if (cart != null)
            {
                var productToRemove = cart.FirstOrDefault(p => p.Evento.EventoID == id);
                if (productToRemove != null)
                {
                    if (productToRemove.QuantitaBiglietti > 1)
                    {
                        productToRemove.QuantitaBiglietti--;
                    }
                    else
                    {
                        cart.Remove(productToRemove);
                    }
                }
            }

            return RedirectToAction("Index");
        }




        // Action per pushare prodotti nel carrello nel db

        [HttpPost]
        public ActionResult Ordina(string note, string indirizzo)
        {
            ModelDbContext db = new ModelDbContext();
            var userId = db.Utenti.FirstOrDefault(u => u.Email == User.Identity.Name).UserID;

            var cart = Session["cart"] as List<CartItem>;
            if (cart != null && cart.Any())
            {
                // Crea un nuovo ordine
                Ordini newOrder = new Ordini();
                newOrder.DataOraOrdine = DateTime.Now;
                newOrder.isEvaso = false;
                newOrder.UserID = userId;

                // Salva l'ordine nel database per ottenere un ID
                db.Ordini.Add(newOrder);
                db.SaveChanges();

                // Per ogni prodotto nel carrello, crea un biglietto e un dettaglio dell'ordine
                foreach (var product in cart)
                {
                    var eventi = db.Eventi.Find(product.Evento.EventoID);
                    if (eventi != null)
                    {
                        if (eventi.BigliettiRimanenti >= product.QuantitaBiglietti)
                        {
                            // Aggiorna il numero di biglietti rimanenti
                            eventi.BigliettiRimanenti -= product.QuantitaBiglietti;
                            db.Entry(eventi).State = EntityState.Modified;

                            // Crea un nuovo biglietto
                            Biglietti newTicket = new Biglietti();
                            newTicket.EventoID = product.Evento.EventoID;
                            newTicket.UserID = userId;
                            newTicket.PrezzoAcquisto = product.Evento.PrezzoBase;
                            newTicket.DataOraAcquisto = DateTime.Now;
                            newTicket.Stato = "Acquistato";
                            newTicket.Quantita = product.QuantitaBiglietti;

                            db.Biglietti.Add(newTicket);
                            db.SaveChanges();

                            // Crea un nuovo dettaglio ordine
                            DettagliOrdine newOrderDetail = new DettagliOrdine();
                            newOrderDetail.OrdineID = newOrder.OrdineID;
                            newOrderDetail.BigliettoID = newTicket.BigliettoID;
                            newOrderDetail.Quantità = product.QuantitaBiglietti;
                            newOrderDetail.PrezzoPerUnità = product.Evento.PrezzoBase;
                            newOrderDetail.Note = note;

                            db.DettagliOrdine.Add(newOrderDetail);
                            db.SaveChanges();
                        }
                        else
                        {
                            // Non ci sono abbastanza biglietti, gestisci l'errore
                            TempData["ErrorMessage"] = "Non ci sono abbastanza biglietti disponibili per l'evento " + eventi.Titolo;
                            return View(cart);
                        }
                    }
                }

                // Calcola il totale dell'ordine
                decimal totale = cart.Sum(product => product.QuantitaBiglietti * product.Evento.PrezzoBase);

                // Aggiorna l'ordine con il totale
                newOrder.Totale = totale;
                db.Entry(newOrder).State = EntityState.Modified;
                db.SaveChanges();

                // Svuota il carrello
                Session["cart"] = null;
            }

            TempData["CreateMess"] = "L'ordine è stato inviato correttamente";
            return RedirectToAction("Index", "Ordini");
        }





        // Svuota il carrello
        public ActionResult CartClear()
        {
            var cart = Session["cart"] as List<CartItem>;
            if (cart != null)
            {
                cart.Clear();
            }
            TempData["CreateMess"] = "Il carrello è stato svuotato";
            return RedirectToAction("Index", "Eventi");
        }

    }
}