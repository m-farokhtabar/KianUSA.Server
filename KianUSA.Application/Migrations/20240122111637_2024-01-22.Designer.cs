﻿// <auto-generated />
using System;
using KianUSA.Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KianUSA.Application.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20240122111637_2024-01-22")]
    partial class _20240122
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.16")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("KianUSA.Domain.Entity.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("Parameter")
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<int>("PublishedCatalogType")
                        .HasColumnType("integer");

                    b.Property<string>("Security")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("ShortDescription")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Tags")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("Order");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.CategoryCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<string>("CategorySlug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<Guid>("ParentCategoryId")
                        .HasColumnType("uuid");

                    b.Property<string>("ParentCategorySlug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("CategoryCategory");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.CategoryProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<string>("CategorySlug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("CategoryProduct");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Filter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Groups")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Order");

                    b.ToTable("Filter");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsVisible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Group");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.PoData", b =>
                {
                    b.Property<string>("PoNumber")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<DateTime?>("BillDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("BookingDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ConfirmDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DischargeStatus")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DocumentsSendOutDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ETA")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ETD")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("EmptyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("FactoryBookingDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FactoryContainerNumber")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int?>("FactoryStatus")
                        .HasColumnType("integer");

                    b.Property<int?>("ForwarderName")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("GateIn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("GateOut")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<string>("PortOfDischarge")
                        .HasColumnType("text");

                    b.Property<double?>("Rate")
                        .HasColumnType("double precision");

                    b.Property<int?>("ShippmentStatus")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("StatusDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("PoNumber");

                    b.ToTable("PoData");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("BoxD")
                        .HasColumnType("double precision");

                    b.Property<double?>("BoxH")
                        .HasColumnType("double precision");

                    b.Property<double?>("BoxW")
                        .HasColumnType("double precision");

                    b.Property<string>("ComplexItemPieces")
                        .HasColumnType("text");

                    b.Property<int>("ComplexItemPriority")
                        .HasColumnType("integer");

                    b.Property<double?>("Cube")
                        .HasColumnType("double precision");

                    b.Property<double?>("D")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Factories")
                        .HasColumnType("text");

                    b.Property<string>("Features")
                        .HasColumnType("text");

                    b.Property<string>("Groups")
                        .HasColumnType("text");

                    b.Property<double?>("H")
                        .HasColumnType("double precision");

                    b.Property<double?>("Inventory")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsGroup")
                        .HasColumnType("boolean");

                    b.Property<string>("IsSample")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<int>("PiecesCount")
                        .HasColumnType("integer");

                    b.Property<string>("Price")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("PricePermissions")
                        .HasColumnType("text");

                    b.Property<string>("ProductDescription")
                        .HasColumnType("text");

                    b.Property<string>("Security")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("ShortDescription")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Tags")
                        .HasColumnType("text");

                    b.Property<double?>("W")
                        .HasColumnType("double precision");

                    b.Property<string>("WHQTY")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<double?>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("Order");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Product");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Buttons")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Pages")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("LastName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Rep")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("ShippingAddress1")
                        .HasMaxLength(220)
                        .HasColumnType("character varying(220)");

                    b.Property<string>("ShippingAddress2")
                        .HasMaxLength(220)
                        .HasColumnType("character varying(220)");

                    b.Property<string>("ShippingCity")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ShippingCountry")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ShippingState")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ShippingZipCode")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("StoreAddress1")
                        .HasMaxLength(220)
                        .HasColumnType("character varying(220)");

                    b.Property<string>("StoreAddress2")
                        .HasMaxLength(220)
                        .HasColumnType("character varying(220)");

                    b.Property<string>("StoreCity")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("StoreCountry")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("StoreName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("StoreState")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("StoreZipCode")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TaxId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.CategoryCategory", b =>
                {
                    b.HasOne("KianUSA.Domain.Entity.Category", null)
                        .WithMany("Parents")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KianUSA.Domain.Entity.Category", null)
                        .WithMany()
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.CategoryProduct", b =>
                {
                    b.HasOne("KianUSA.Domain.Entity.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KianUSA.Domain.Entity.Product", null)
                        .WithMany("Categories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.UserRole", b =>
                {
                    b.HasOne("KianUSA.Domain.Entity.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KianUSA.Domain.Entity.User", null)
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Category", b =>
                {
                    b.Navigation("Parents");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.Product", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("KianUSA.Domain.Entity.User", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}