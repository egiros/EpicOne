using EpicOne.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.Globalization;

namespace EpicOne.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {

        public ActionResult Index(int? categoriaId)
        {
            ViewBag.IsHome = true;

            if (Session["LangCode"] == null)
            {
                Session["LangCode"] = "IT";
                Session["FlagImage"] = Url.Content("~/Content/Img/it.png");
            }

            using (var db = new ModelDbContext())
            {
                var model = new HomeIndexViewModel
                {
                    Categorie = db.Categorie
                    .Include(c => c.Eventi)
                    .ToList()
                };

                if (categoriaId.HasValue)
                {
                    model.Eventi = db.Eventi
                        .Include(e => e.Categorie)
                        .Include(e => e.Luoghi)
                        .Include(e => e.Utenti)
                        .Where(e => e.CategoriaID == categoriaId)
                        .OrderByDescending(e => e.DataOra)
                        .ToList();
                    if (model.Eventi.Any())
                    {
                        model.CategoriaSelezionata = db.Categorie.Find(categoriaId);
                    }
                }
                else
                {
                    model.Eventi = db.Eventi
                        .Include(e => e.Categorie)
                        .Include(e => e.Luoghi)
                        .Include(e => e.Utenti)
                        .OrderByDescending(e => e.DataOra)
                        .ToList();
                }


                return View(model);
            }
        }


        public ActionResult DefaultEvents()
        {
            using (var db = new ModelDbContext())
            {
                var model = db.Eventi
                    .Include(e => e.Categorie)
                    .Include(e => e.Luoghi)
                    .Include(e => e.Utenti)
                    .OrderByDescending(e => e.DataOra)
                    .Take(8) // Limita a 10 eventi
                    .ToList();

                return PartialView("~/Views/Eventi/Index.cshtml", model);
            }
        }

        public ActionResult EventiPerCategoria()
        {
            using (var db = new ModelDbContext())
            {
                var nomiCategorie = new List<string> { "Musica", "Discoteca" };
                var model = new HomeIndexViewModel();

                model.Categorie = db.Categorie
                     .Include(c => c.Eventi.Select(e => e.Luoghi))
                     .Where(c => nomiCategorie.Contains(c.Nome))
                     .ToList();

                foreach (var categoria in model.Categorie)
                {
                    categoria.Eventi = categoria.Eventi.Take(5).ToList(); // Limita a 5 eventi per categoria
                }

                return PartialView("~/Views/Categorie/Index.cshtml", model.Categorie);
            }
        }


        public ActionResult Search(string searchQuery)
        {
            var lowerCaseSearchQuery = searchQuery.ToLower();
            using (var db = new ModelDbContext())
            {
                var eventi = db.Eventi
                    .Include(e => e.Luoghi)
                    .Where(e => e.Titolo.ToLower().Contains(lowerCaseSearchQuery)
                                || e.Descrizione.ToLower().Contains(lowerCaseSearchQuery)
                                || e.Luoghi.Nome.ToLower().Contains(lowerCaseSearchQuery))
                    .ToList();
                return PartialView("_SearchResults", eventi);
            }
        }

        public ActionResult Login()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            using (var context = new ModelDbContext())
            {
                
                var user = context.Utenti.FirstOrDefault(u => u.Email == Email && u.Password == Password);
                
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(Email, false);
                    TempData["LoginMessage"] = "Login Effettuato Correttamente";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Credenziali non valide. Riprova.");
                    return View();
                }
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Register(Utenti utente)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ModelDbContext())
                {
                    context.Utenti.Add(utente);
                    context.SaveChanges();
                    TempData["RegisterMessage"] = "Utente registrato con successo";
                }

                return RedirectToAction("Login", "Home");
            }

            return View(utente);
        }

        public ActionResult ChangeLanguage(string lang, string langCode, string flagImage, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(lang);
            Session["LangCode"] = langCode;
            Session["FlagImage"] = flagImage;
            return Redirect(returnUrl);
        }

        public ActionResult ChiSiamo()
        {
            return View();
        }



    }
}