using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategory>
{
    public void Configure(EntityTypeBuilder<TicketCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        // Seed data
        builder.HasData(
            new TicketCategory { Id = 1, Name = "Web Kurulum", Description = "Web sitesi kurulum talepleri", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 2, Name = "Revize Talebi", Description = "Mevcut web sitesi değişiklik talepleri", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 3, Name = "Grafik Tasarım", Description = "Grafik tasarım talepleri", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 4, Name = "Yazılım", Description = "Yazılım geliştirme talepleri", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 5, Name = "Alibaba.com", Description = "Alibaba.com ile ilgili talepler", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 6, Name = "Muhasebe Satış", Description = "Muhasebe ve satış talepleri", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 7, Name = "Alan Adı Sunucu Lisans Yenileme", Description = "Alan adı, sunucu ve lisans yenileme", CreatedAt = DateTime.UtcNow },
            new TicketCategory { Id = 8, Name = "Sunucu-Email Yönetimi Teknik Destek", Description = "Sunucu ve email yönetimi", CreatedAt = DateTime.UtcNow }
        );
    }
}
