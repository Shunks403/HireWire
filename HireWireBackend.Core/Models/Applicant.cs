using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public partial class Applicant
{
    [Key]
    public int ApplicantId { get; set; }

    public string? Resume { get; set; }

    public string? Skills { get; set; }

    public string? Education { get; set; }

    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("ApplicantId")]
    [InverseProperty("Applicant")]
    public virtual User ApplicantNavigation { get; set; } = null!;

    [InverseProperty("Applicant")]
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
