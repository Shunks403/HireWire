using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


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
    
    [Column(TypeName = "nvarchar(512)")]
    public string? RefreshToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [InverseProperty("Admin")]
    [JsonIgnore]
    public virtual ICollection<AdminAction> AdminActions { get; set; } = new List<AdminAction>();

    [InverseProperty("ApplicantNavigation")]
    [JsonIgnore]
    public virtual Applicant? Applicant { get; set; }

    [InverseProperty("EmployerNavigation")]
    [JsonIgnore]
    public virtual Employer? Employer { get; set; }
}
