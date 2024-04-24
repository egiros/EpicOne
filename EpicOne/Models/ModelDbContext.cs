using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EpicOne.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<Biglietti> Biglietti { get; set; }
        public virtual DbSet<Categorie> Categorie { get; set; }
        public virtual DbSet<DettagliOrdine> DettagliOrdine { get; set; }
        public virtual DbSet<Eventi> Eventi { get; set; }
        public virtual DbSet<Luoghi> Luoghi { get; set; }
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Biglietti>()
                .Property(e => e.PrezzoAcquisto)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Biglietti>()
                .HasMany(e => e.DettagliOrdine)
                .WithRequired(e => e.Biglietti)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DettagliOrdine>()
                .Property(e => e.PrezzoPerUnità)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Eventi>()
                .Property(e => e.PrezzoBase)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Eventi>()
                .HasMany(e => e.Biglietti)
                .WithRequired(e => e.Eventi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ordini>()
                .Property(e => e.Totale)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ordini>()
                .HasMany(e => e.DettagliOrdine)
                .WithRequired(e => e.Ordini)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Eventi)
                .WithOptional(e => e.Utenti)
                .HasForeignKey(e => e.OrganizzatoreID);

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Ordini)
                .WithRequired(e => e.Utenti)
                .WillCascadeOnDelete(false);
        }
    }
}
