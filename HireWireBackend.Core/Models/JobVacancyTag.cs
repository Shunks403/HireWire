using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace HireWireBackend.Core.Models;

public partial class JobVacancyTag
{
    [Key]
    public int Id { get; set; }

    public int VacancyId { get; set; }

    public int TagId { get; set; }

    [ForeignKey("VacancyId")]
    [JsonIgnore]
    public virtual JobVacancy Vacancy { get; set; }

    [ForeignKey("TagId")]
    [JsonIgnore]
    public virtual Tag Tag { get; set; }
}