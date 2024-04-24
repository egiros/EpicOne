namespace EpicOne.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Biglietti")]
    public partial class Biglietti
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Biglietti()
        {
            DettagliOrdine = new HashSet<DettagliOrdine>();
        }

        [Key]
        public int BigliettoID { get; set; }

        public int EventoID { get; set; }

        public int? UserID { get; set; }

        public decimal PrezzoAcquisto { get; set; }

        public DateTime DataOraAcquisto { get; set; }

        [Required]
        [StringLength(255)]
        public string Stato { get; set; }

        public int Quantita { get; set; }

        public virtual Eventi Eventi { get; set; }

        public virtual Utenti Utenti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettagliOrdine> DettagliOrdine { get; set; }
    }
}
