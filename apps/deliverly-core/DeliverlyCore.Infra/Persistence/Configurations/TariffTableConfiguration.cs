using DeliverlyCore.Pricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliverlyCore.Infra.Persistence.Configurations
{
    public class TariffTableConfiguration : IEntityTypeConfiguration<TariffTable>
    {
        public void Configure(EntityTypeBuilder<TariffTable> builder)
        {
            builder.ToTable("tariff");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                   .HasColumnName("id");

            builder.Property(t => t.CreatedAt)
                   .HasColumnName("created_at");

            builder.Property(t => t.Description)
                   .HasColumnName("description")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.OwnsOne(t => t.OriginPrefix, zip =>
            {
                zip.Property(z => z.Value)
                   .HasColumnName("origin_prefix")
                   .HasMaxLength(8)
                   .IsRequired();
            });

            builder.OwnsOne(t => t.DestinationPrefix, zip =>
            {
                zip.Property(z => z.Value)
                   .HasColumnName("destination_prefix")
                   .HasMaxLength(8)
                   .IsRequired();
            });

            builder.OwnsOne(t => t.MinWeight, w =>
            {
                w.Property(x => x.Value)
                 .HasColumnName("min_weight")
                 .HasColumnType("numeric(10,3)")
                 .IsRequired();
            });

            builder.OwnsOne(t => t.MaxWeight, w =>
            {
                w.Property(x => x.Value)
                 .HasColumnName("max_weight")
                 .HasColumnType("numeric(10,3)")
                 .IsRequired();
            });

            builder.OwnsOne(t => t.BaseValue, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("base_value_amount")
                     .HasColumnType("numeric(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("base_value_currency")
                     .HasMaxLength(3)
                     .IsRequired();
            });
        }
    }
}
