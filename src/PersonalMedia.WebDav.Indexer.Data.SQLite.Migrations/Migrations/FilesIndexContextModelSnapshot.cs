// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalMedia.WebDav.Indexer.Data.SQLite;

#nullable disable

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Migrations.Migrations
{
    [DbContext(typeof(FilesIndexContext))]
    partial class FilesIndexContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0");

            modelBuilder.Entity("PersonalMedia.WebDav.Indexer.Data.SQLite.Entity.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<long?>("Created")
                        .HasColumnType("INTEGER")
                        .HasColumnName("created");

                    b.Property<string>("ETag")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("etag");

                    b.Property<int?>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("Modified")
                        .HasColumnType("INTEGER")
                        .HasColumnName("modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("path");

                    b.Property<long?>("Size")
                        .HasColumnType("INTEGER")
                        .HasColumnName("size");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("item_type");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("Items", (string)null);
                });

            modelBuilder.Entity("PersonalMedia.WebDav.Indexer.Data.SQLite.Entity.Item", b =>
                {
                    b.HasOne("PersonalMedia.WebDav.Indexer.Data.SQLite.Entity.Item", null)
                        .WithMany("Children")
                        .HasForeignKey("ItemId");
                });

            modelBuilder.Entity("PersonalMedia.WebDav.Indexer.Data.SQLite.Entity.Item", b =>
                {
                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}
