using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace HireWireBackend.Core.Models;

public partial class JobVacancyTag
{
    [Key]
    public int Id { get; set; }

    public int VacancyId { get; set; }

    public int TagId { get; set; }

    [ForeignKey("VacancyId")]
    public virtual JobVacancy Vacancy { get; set; }

    [ForeignKey("TagId")]
    public virtual Tag Tag { get; set; }
}