﻿// <auto-generated />
using System;
using HireWireBackend.Storage.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HireWireBackend.Storage.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class AplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AdminAction", b =>
                {
                    b.Property<int>("ActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActionId"));

                    b.Property<string>("ActionDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("AdminId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int?>("TargetUserId")
                        .HasColumnType("int");

                    b.Property<int?>("TargetVacancyId")
                        .HasColumnType("int");

                    b.HasKey("ActionId")
                        .HasName("PK__AdminAct__FFE3F4D97C7A66F2");

                    b.HasIndex("AdminId");

                    b.ToTable("AdminActions");
                });

            modelBuilder.Entity("Applicant", b =>
                {
                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Education")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Resume")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Skills")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("ApplicantId")
                        .HasName("PK__Applican__39AE91A8CA19FBD7");

                    b.ToTable("Applicants");
                });

            modelBuilder.Entity("Employer", b =>
                {
                    b.Property<int>("EmployerId")
                        .HasColumnType("int");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ContactInfo")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("EmployerId")
                        .HasName("PK__Employer__CA4452614D9889C1");

                    b.ToTable("Employers");
                });

            modelBuilder.Entity("JobApplication", b =>
                {
                    b.Property<int>("ApplicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicationId"));

                    b.Property<int?>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.Property<int?>("VacancyId")
                        .HasColumnType("int");

                    b.HasKey("ApplicationId")
                        .HasName("PK__JobAppli__C93A4C99CA02451B");

                    b.HasIndex("ApplicantId");

                    b.HasIndex("VacancyId");

                    b.ToTable("JobApplications");
                });

            modelBuilder.Entity("JobVacancy", b =>
                {
                    b.Property<int>("VacancyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VacancyId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EmployerId")
                        .HasColumnType("int");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Requirements")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Salary")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("VacancyId")
                        .HasName("PK__JobVacan__6456763FAE986CFF");

                    b.HasIndex("EmployerId");

                    b.ToTable("JobVacancies");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Avatar")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CC4CB8F5A4F4");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AdminAction", b =>
                {
                    b.HasOne("User", "Admin")
                        .WithMany("AdminActions")
                        .HasForeignKey("AdminId")
                        .HasConstraintName("FK__AdminActi__Admin__534D60F1");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Applicant", b =>
                {
                    b.HasOne("User", "ApplicantNavigation")
                        .WithOne("Applicant")
                        .HasForeignKey("Applicant", "ApplicantId")
                        .IsRequired()
                        .HasConstraintName("FK__Applicant__Appli__3D5E1FD2");

                    b.Navigation("ApplicantNavigation");
                });

            modelBuilder.Entity("Employer", b =>
                {
                    b.HasOne("User", "EmployerNavigation")
                        .WithOne("Employer")
                        .HasForeignKey("Employer", "EmployerId")
                        .IsRequired()
                        .HasConstraintName("FK__Employers__Emplo__4222D4EF");

                    b.Navigation("EmployerNavigation");
                });

            modelBuilder.Entity("JobApplication", b =>
                {
                    b.HasOne("Applicant", "Applicant")
                        .WithMany("JobApplications")
                        .HasForeignKey("ApplicantId")
                        .HasConstraintName("FK__JobApplic__Appli__4D94879B");

                    b.HasOne("JobVacancy", "Vacancy")
                        .WithMany("JobApplications")
                        .HasForeignKey("VacancyId")
                        .HasConstraintName("FK__JobApplic__Vacan__4CA06362");

                    b.Navigation("Applicant");

                    b.Navigation("Vacancy");
                });

            modelBuilder.Entity("JobVacancy", b =>
                {
                    b.HasOne("Employer", "Employer")
                        .WithMany("JobVacancies")
                        .HasForeignKey("EmployerId")
                        .HasConstraintName("FK__JobVacanc__Emplo__46E78A0C");

                    b.Navigation("Employer");
                });

            modelBuilder.Entity("Applicant", b =>
                {
                    b.Navigation("JobApplications");
                });

            modelBuilder.Entity("Employer", b =>
                {
                    b.Navigation("JobVacancies");
                });

            modelBuilder.Entity("JobVacancy", b =>
                {
                    b.Navigation("JobApplications");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("AdminActions");

                    b.Navigation("Applicant");

                    b.Navigation("Employer");
                });
#pragma warning restore 612, 618
        }
    }
}