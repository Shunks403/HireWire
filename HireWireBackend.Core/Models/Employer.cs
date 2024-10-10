using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public partial class Employer
{
    [Key]
    public int EmployerId { get; set; }

    [StringLength(255)]
    public string? CompanyName { get; set; }

    [StringLength(255)]
    public string? ContactInfo { get; set; }

    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("EmployerId")]
    [InverseProperty("Employer")]
    public virtual User EmployerNavigation { get; set; } = null!;

    [InverseProperty("Employer")]
    public virtual ICollection<JobVacancy> JobVacancies { get; set; } = new List<JobVacancy>();
}
