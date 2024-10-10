using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Storage.Context;

public partial class ApplicationDbContext : DbContext
{
    

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminAction> AdminActions { get; set; }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<JobVacancy> JobVacancies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__AdminAct__FFE3F4D97C7A66F2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminActions).HasConstraintName("FK__AdminActi__Admin__534D60F1");
        });

        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.ApplicantId).HasName("PK__Applican__39AE91A8CA19FBD7");

            entity.Property(e => e.ApplicantId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.ApplicantNavigation).WithOne(p => p.Applicant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Applicant__Appli__3D5E1FD2");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.EmployerId).HasName("PK__Employer__CA4452614D9889C1");

            entity.Property(e => e.EmployerId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.EmployerNavigation).WithOne(p => p.Employer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employers__Emplo__4222D4EF");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__JobAppli__C93A4C99CA02451B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Applicant).WithMany(p => p.JobApplications).HasConstraintName("FK__JobApplic__Appli__4D94879B");

            entity.HasOne(d => d.Vacancy).WithMany(p => p.JobApplications).HasConstraintName("FK__JobApplic__Vacan__4CA06362");
        });

        modelBuilder.Entity<JobVacancy>(entity =>
        {
            entity.HasKey(e => e.VacancyId).HasName("PK__JobVacan__6456763FAE986CFF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Employer).WithMany(p => p.JobVacancies).HasConstraintName("FK__JobVacanc__Emplo__46E78A0C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CB8F5A4F4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
