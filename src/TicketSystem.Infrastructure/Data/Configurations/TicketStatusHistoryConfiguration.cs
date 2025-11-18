using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
{
    public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
    {
        builder.ToTable("TicketStatusHistories");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.ChangedBy)
            .IsRequired();

        builder.Property(t => t.Comment)
            .HasMaxLength(1000);

        // Configure relationship with User - explicitly set ChangedBy as foreign key
        builder.HasOne(t => t.ChangedByUser)
            .WithMany()
            .HasForeignKey(t => t.ChangedBy)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Note: Ticket relationship is configured in TicketConfiguration.cs
    }
}
