﻿// <auto-generated />
using System;
using IwaraDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IwaraDatabase.Migrations
{
    [DbContext(typeof(Database))]
    [Migration("20211026115700_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.20");

            modelBuilder.Entity("IwaraDatabase.MMDInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EyeOpen")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<int>("Heart")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MonthInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<string>("UnixTimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.Property<bool>("WhetherDownloaded")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MonthInfoId");

                    b.ToTable("MMDInfos");
                });

            modelBuilder.Entity("IwaraDatabase.MonthInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Month")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("MonthInfos");
                });

            modelBuilder.Entity("IwaraDatabase.MMDInfo", b =>
                {
                    b.HasOne("IwaraDatabase.MonthInfo", null)
                        .WithMany("MMDs")
                        .HasForeignKey("MonthInfoId");
                });
#pragma warning restore 612, 618
        }
    }
}
