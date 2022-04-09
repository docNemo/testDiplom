using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataContext
{
    public partial class diplomContext : DbContext
    {
        public diplomContext()
        {
        }

        public diplomContext(DbContextOptions<diplomContext> options)
            : base(options)
        {
        }

        public virtual DbSet<NameInRepresentation> NameInRepresentations { get; set; } = null!;
        public virtual DbSet<NameOntologyPredicate> NameOntologyPredicates { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConfigurationManager.ConnectionStrings["DiplomDB"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NameInRepresentation>(entity =>
            {
                entity.HasKey(e => e.IdNameRepr)
                    .HasName("name_in_representation_pkey");

                entity.ToTable("name_in_representation");

                entity.Property(e => e.IdNameRepr)
                    .HasColumnName("id_name_repr")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.NameRepr)
                    .HasMaxLength(50)
                    .HasColumnName("name_repr");
            });

            modelBuilder.Entity<NameOntologyPredicate>(entity =>
            {
                entity.HasKey(e => e.IdConnection)
                    .HasName("name_ontology_predicate_pkey");

                entity.ToTable("name_ontology_predicate");

                entity.Property(e => e.IdConnection)
                    .HasColumnName("id_connection")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IdNameRepr).HasColumnName("id_name_repr");

                entity.Property(e => e.NamePredicate)
                    .HasMaxLength(50)
                    .HasColumnName("name_predicate");

                entity.HasOne(d => d.IdNameReprNavigation)
                    .WithMany(p => p.NameOntologyPredicates)
                    .HasForeignKey(d => d.IdNameRepr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("name_ontology_predicate_id_name_repr_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
