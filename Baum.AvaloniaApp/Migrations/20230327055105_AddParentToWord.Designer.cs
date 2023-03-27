﻿// <auto-generated />
using System;
using Baum.AvaloniaApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Baum.AvaloniaApp.Migrations
{
    [DbContext(typeof(ProjectContext))]
    [Migration("20230327055105_AddParentToWord")]
    partial class AddParentToWord
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("Baum.AvaloniaApp.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SoundChange")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Baum.AvaloniaApp.Models.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IPA")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LanguageId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.HasIndex("ParentId");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("Baum.AvaloniaApp.Models.Language", b =>
                {
                    b.HasOne("Baum.AvaloniaApp.Models.Language", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Baum.AvaloniaApp.Models.Word", b =>
                {
                    b.HasOne("Baum.AvaloniaApp.Models.Language", "Language")
                        .WithMany("Words")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Baum.AvaloniaApp.Models.Word", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Language");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Baum.AvaloniaApp.Models.Language", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Words");
                });
#pragma warning restore 612, 618
        }
    }
}
