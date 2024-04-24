namespace EpicOne.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DettagliOrdine")]
    public partial class DettagliOrdine
    {
        [Key]
        public int DettaglioOrdineID { get; set; }

        public int OrdineID { get; set; }

        public int BigliettoID { get; set; }

        public int Quantità { get; set; }

        public decimal PrezzoPerUnità { get; set; }

        public string Note { get; set; }

        public virtual Biglietti Biglietti { get; set; }

        public virtual Ordini Ordini { get; set; }
    }
}
