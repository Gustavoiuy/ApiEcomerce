using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    public class ReservationEntityConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.TableNumber).IsRequired();
            builder.Property(r => r.CustomerName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.CustomerPhone).IsRequired().HasMaxLength(20);
            builder.Property(r => r.ReservationDate).IsRequired();
            builder.Property(r => r.NumberOfPeople).IsRequired();
            builder.Property(r => r.Notes).HasMaxLength(250);
            builder.Property(r => r.HasPets).IsRequired();
            builder.Property(r => r.Email).IsRequired().HasMaxLength(150);
        }
    }
}