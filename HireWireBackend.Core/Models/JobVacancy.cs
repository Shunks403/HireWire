using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




public partial class JobVacancy
{
    [Key]
    public int VacancyId { get; set; }

    public int? EmployerId { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Requirements { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Salary { get; set; }

    [StringLength(255)]
    public string? Location { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("EmployerId")]
    [InverseProperty("JobVacancies")]
    public virtual Employer? Employer { get; set; }

    [InverseProperty("Vacancy")]
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
