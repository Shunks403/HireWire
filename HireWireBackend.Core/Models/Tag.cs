using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HireWireBackend.Core.Models;

public partial class Tag
{
    [Key]
    public int TagId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [InverseProperty("Tag")]
    [JsonIgnore]
    public virtual ICollection<JobVacancyTag> JobVacancyTags { get; set; } = new List<JobVacancyTag>();
}