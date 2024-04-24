using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EpicOne.Models;

namespace EpicOne.Models
{
    public class HomeIndexViewModel
    {
        public List<Eventi> Eventi { get; set; }
        public List<Categorie> Categorie { get; set; }
        public Categorie CategoriaSelezionata { get; set; }
    }
}