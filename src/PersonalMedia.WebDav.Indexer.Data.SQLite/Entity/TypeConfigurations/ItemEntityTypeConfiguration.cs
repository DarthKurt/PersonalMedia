using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Entity.TypeConfigurations;

public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder
            .ToTable("Items")
            .HasKey(k => k.Id);

        builder
            .Property(b => b.Id)
            .HasColumnName("id");

        builder
            .Property(b => b.Name)
            .HasColumnName("name");

        builder
            .Property(b => b.Path)
            .HasColumnName("path");

        builder
            .Property(b => b.ETag)
            .HasColumnName("etag");

        builder
            .Property(b => b.Created)
            .HasColumnName("created")
            .HasConversion(new DateTimeToBinaryConverter());

        builder
            .Property(b => b.Modified)
            .HasColumnName("modified")
            .HasConversion(new DateTimeToBinaryConverter());

        builder
            .Property(b => b.Size)
            .HasColumnName("size");

        builder
            .Property(b => b.Type)
            .HasColumnName("item_type")
            .HasConversion(new EnumToStringConverter<ItemType>());
    }
}
