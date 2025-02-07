﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Orchestrator.StateMachine.Database;

#nullable disable

namespace Orchestrator.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Orchestrator.StateMachine.Core.CascadingCommunicationState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<long>("CommunicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("DeliveryChannel")
                        .HasColumnType("integer");

                    b.Property<int>("PushDeliveryTimeoutSeconds")
                        .HasColumnType("integer");

                    b.Property<int>("PushSendTimeoutSeconds")
                        .HasColumnType("integer");

                    b.Property<uint>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.Property<int>("SmsDeliveryTimeoutDays")
                        .HasColumnType("integer");

                    b.Property<bool>("Success")
                        .HasColumnType("boolean");

                    b.ComplexProperty<Dictionary<string, object>>("SmsData", "Orchestrator.StateMachine.Core.CascadingCommunicationState.SmsData#SmsData", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int>("Priority")
                                .HasColumnType("integer");

                            b1.Property<string>("Text")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<long>("To")
                                .HasColumnType("bigint");
                        });

                    b.HasKey("CorrelationId");

                    b.HasIndex("CommunicationId");

                    b.ToTable("CascadingCommunicationState");
                });
#pragma warning restore 612, 618
        }
    }
}
