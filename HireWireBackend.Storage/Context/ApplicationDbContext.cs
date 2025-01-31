using System;
using System.Collections.Generic;
using HireWireBackend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Storage.Context;

public partial class ApplicationDbContext : DbContext
{
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-QSKA3AR;Database=HireWire;Trusted_Connection=True;TrustServerCertificate=True");
    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    
    
    public virtual DbSet<AdminAction> AdminActions { get; set; }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<JobVacancy> JobVacancies { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<Tag> Tags { get; set; }
    
    public virtual DbSet<JobVacancyTag> JobVacancyTags { get; set; }
   
    
    

    

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
            entity.HasIndex(e => new { e.VacancyId, e.ApplicantId })
                .IsUnique()
                .HasDatabaseName("IX_Unique_JobApplication");
        });

        modelBuilder.Entity<JobVacancy>(entity =>
        {
            entity.HasKey(e => e.VacancyId).HasName("PK__JobVacan__6456763FAE986CFF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Employer).WithMany(p => p.JobVacancies).HasConstraintName("FK__JobVacanc__Emplo__46E78A0C");
            
            entity.HasMany(jv => jv.JobVacancyTags)
                .WithOne(jvt => jvt.Vacancy)
                .HasForeignKey(jvt => jvt.VacancyId)
                .HasConstraintName("FK__JobVacanc__Vacan__47DBAE45");
        });
        
        modelBuilder.Entity<JobVacancyTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobVacan__3214EC07123F0EEC");

            entity.HasOne(jvt => jvt.Vacancy)
                .WithMany(jv => jv.JobVacancyTags)
                .HasForeignKey(jvt => jvt.VacancyId)
                .HasConstraintName("FK__JobVacanc__Vacan__48CFD27E");

            entity.HasOne(jvt => jvt.Tag)
                .WithMany(t => t.JobVacancyTags)
                .HasForeignKey(jvt => jvt.TagId)
                .HasConstraintName("FK__JobVacanc__TagId__49C3F6B7");
        });
        
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__D5B6E3B69F1A1BA7");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
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
