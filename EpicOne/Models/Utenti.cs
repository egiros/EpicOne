namespace EpicOne.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Entity.Spatial;

    [Table("Utenti")]
    public partial class Utenti
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utenti()
        {
            Biglietti = new HashSet<Biglietti>();
            Eventi = new HashSet<Eventi>();
            Ordini = new HashSet<Ordini>();
        }

        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; }

        [Required]
        [StringLength(200)]
        public string Cognome { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public DateTime DataRegistrazione { get; set; } = DateTime.Now;

        [Required]
        public bool IsAdmin { get; set; }

        [StringLength(255)]
        public string Indirizzo { get; set; }

        [StringLength(100)]
        public string Città { get; set; }

        [StringLength(20)]
        public string CodicePostale { get; set; }

        [StringLength(100)]
        public string Paese { get; set; }

        [StringLength(20)]
        public string NumeroDiTelefono { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DataDiNascita { get; set; }

        public string FotoProfilo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Biglietti> Biglietti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Eventi> Eventi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ordini> Ordini { get; set; }
    }
}
