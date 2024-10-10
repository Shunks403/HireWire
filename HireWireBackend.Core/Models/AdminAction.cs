using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public partial class AdminAction
{
    [Key]
    public int ActionId { get; set; }

    public int? AdminId { get; set; }

    public string? ActionDescription { get; set; }

    public int? TargetUserId { get; set; }

    public int? TargetVacancyId { get; set; }

    public bool? IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("AdminActions")]
    public virtual User? Admin { get; set; }
}
