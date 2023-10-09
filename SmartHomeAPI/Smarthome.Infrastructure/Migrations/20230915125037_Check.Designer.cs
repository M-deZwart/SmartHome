﻿// <auto-generated />
using System;
using Smarthome.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(SmartHomeContext))]
    [Migration("20230915125037_Check")]
    partial class Check
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.8");

            modelBuilder.Entity("Domain.Domain.Entities.Humidity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT")
                        .HasColumnName("Date");

                    b.Property<double>("Percentage")
                        .HasColumnType("REAL")
                        .HasColumnName("Percentage");

                    b.Property<Guid>("SensorId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("Humidities", (string)null);
                });

            modelBuilder.Entity("Domain.Domain.Entities.Sensor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sensors");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4b1afb39-0e95-4c6c-95cb-3551cdc599be"),
                            Title = "LivingRoom"
                        },
                        new
                        {
                            Id = new Guid("98690b30-1b87-4870-9939-a79f9c198625"),
                            Title = "Bedroom"
                        },
                        new
                        {
                            Id = new Guid("d2595ce4-ecb5-441e-a59f-f7628b349281"),
                            Title = "WorkSpace"
                        });
                });

            modelBuilder.Entity("Domain.Domain.Entities.Temperature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double>("Celsius")
                        .HasColumnType("REAL")
                        .HasColumnName("Celsius");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT")
                        .HasColumnName("Date");

                    b.Property<Guid>("SensorId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("Temperatures", (string)null);
                });

            modelBuilder.Entity("Domain.Domain.Entities.Humidity", b =>
                {
                    b.HasOne("Domain.Domain.Entities.Sensor", "Sensor")
                        .WithMany("Humidities")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("Domain.Domain.Entities.Temperature", b =>
                {
                    b.HasOne("Domain.Domain.Entities.Sensor", "Sensor")
                        .WithMany("Temperatures")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("Domain.Domain.Entities.Sensor", b =>
                {
                    b.Navigation("Humidities");

                    b.Navigation("Temperatures");
                });
#pragma warning restore 612, 618
        }
    }
}