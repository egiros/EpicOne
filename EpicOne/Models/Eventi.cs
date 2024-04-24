namespace EpicOne.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Eventi")]
    public partial class Eventi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Eventi()
        {
            Biglietti = new HashSet<Biglietti>();
        }

        [Key]
        public int EventoID { get; set; }

        [Required]
        [StringLength(200)]
        public string Titolo { get; set; }

        public string Descrizione { get; set; }

        public DateTime DataOra { get; set; }

        public int? LuogoID { get; set; }

        public int? CategoriaID { get; set; }

        public int? OrganizzatoreID { get; set; }

        public decimal PrezzoBase { get; set; }

        public int QuantitàTotaleBiglietti { get; set; }

        public int BigliettiRimanenti { get; set; }

        public string Immagine { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Biglietti> Biglietti { get; set; }

        public virtual Categorie Categorie { get; set; }

        public virtual Luoghi Luoghi { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
