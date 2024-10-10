using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string? Avatar { get; set; }

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(50)]
    public string Role { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<AdminAction> AdminActions { get; set; } = new List<AdminAction>();

    [InverseProperty("ApplicantNavigation")]
    public virtual Applicant? Applicant { get; set; }

    [InverseProperty("EmployerNavigation")]
    public virtual Employer? Employer { get; set; }
}
