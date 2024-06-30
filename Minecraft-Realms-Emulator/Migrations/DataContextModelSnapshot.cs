﻿// <auto-generated />
using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minecraft_Realms_Emulator.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Backup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BackupId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DownloadUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("LastModifiedDate")
                        .HasColumnType("bigint");

                    b.Property<JsonDocument>("Metadata")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("ResourcePackHash")
                        .HasColumnType("text");

                    b.Property<string>("ResourcePackUrl")
                        .HasColumnType("text");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.Property<int>("SlotId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SlotId");

                    b.ToTable("Backups");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Configuration", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<object>("Value")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Key");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Connection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PendingUpdate")
                        .HasColumnType("boolean");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorldId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Invite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("InvitationId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecipeintUUID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorldId");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<JsonDocument>("ButtonText")
                        .HasColumnType("jsonb");

                    b.Property<bool>("Dismissable")
                        .HasColumnType("boolean");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<JsonDocument>("Message")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("NotificationUuid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<JsonDocument>("Title")
                        .HasColumnType("jsonb");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<JsonDocument>("UrlButton")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Online")
                        .HasColumnType("boolean");

                    b.Property<bool>("Operator")
                        .HasColumnType("boolean");

                    b.Property<string>("Permission")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorldId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.SeenNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NotificationUUID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerUUID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SeenNotifications");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Slot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("CommandBlocks")
                        .HasColumnType("boolean");

                    b.Property<int>("Difficulty")
                        .HasColumnType("integer");

                    b.Property<bool>("ForceGameMode")
                        .HasColumnType("boolean");

                    b.Property<int>("GameMode")
                        .HasColumnType("integer");

                    b.Property<bool>("Pvp")
                        .HasColumnType("boolean");

                    b.Property<int>("SlotId")
                        .HasColumnType("integer");

                    b.Property<string>("SlotName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("SpawnAnimals")
                        .HasColumnType("boolean");

                    b.Property<bool>("SpawnMonsters")
                        .HasColumnType("boolean");

                    b.Property<bool>("SpawnNPCs")
                        .HasColumnType("boolean");

                    b.Property<int>("SpawnProtection")
                        .HasColumnType("integer");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorldId");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SubscriptionType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WorldId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorldId")
                        .IsUnique();

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Template", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecommendedPlayers")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Trailer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.World", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ActiveSlot")
                        .HasColumnType("integer");

                    b.Property<int>("MaxPlayers")
                        .HasColumnType("integer");

                    b.Property<bool>("Member")
                        .HasColumnType("boolean");

                    b.Property<int?>("MinigameId")
                        .HasColumnType("integer");

                    b.Property<string>("Motd")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Owner")
                        .HasColumnType("text");

                    b.Property<string>("OwnerUUID")
                        .HasColumnType("text");

                    b.Property<int?>("ParentWorldId")
                        .HasColumnType("integer");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WorldType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MinigameId");

                    b.HasIndex("ParentWorldId");

                    b.ToTable("Worlds");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Backup", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.Slot", "Slot")
                        .WithMany()
                        .HasForeignKey("SlotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Connection", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.World", "World")
                        .WithMany()
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Invite", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.World", "World")
                        .WithMany()
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Player", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.World", "World")
                        .WithMany("Players")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Slot", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.World", "World")
                        .WithMany("Slots")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.Subscription", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.World", "World")
                        .WithOne("Subscription")
                        .HasForeignKey("Minecraft_Realms_Emulator.Entities.Subscription", "WorldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.World", b =>
                {
                    b.HasOne("Minecraft_Realms_Emulator.Entities.Template", "Minigame")
                        .WithMany()
                        .HasForeignKey("MinigameId");

                    b.HasOne("Minecraft_Realms_Emulator.Entities.World", "ParentWorld")
                        .WithMany()
                        .HasForeignKey("ParentWorldId");

                    b.Navigation("Minigame");

                    b.Navigation("ParentWorld");
                });

            modelBuilder.Entity("Minecraft_Realms_Emulator.Entities.World", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("Slots");

                    b.Navigation("Subscription");
                });
#pragma warning restore 612, 618
        }
    }
}
