using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


public partial class JobApplication
{
    [Key]
    public int ApplicationId { get; set; }

    public int? VacancyId { get; set; }

    public int? ApplicantId { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("ApplicantId")]
    [InverseProperty("JobApplications")]
    [JsonIgnore]
    public virtual Applicant? Applicant { get; set; }

    [ForeignKey("VacancyId")]
    [InverseProperty("JobApplications")]
    [JsonIgnore]
    public virtual JobVacancy? Vacancy { get; set; }
}
