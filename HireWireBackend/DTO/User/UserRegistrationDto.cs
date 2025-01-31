using System.ComponentModel.DataAnnotations;

namespace HireWireBackend.DTO;

public class UserRegistrationDto
{
    [Required]
    public String FirstName { get; set; }
    
    [Required]
    public String LastName { get; set; }
    
    [Required]
    public String Email { get; set; }
    
    [Required]
    public bool IsEmployer { get; set; }
    
    [Required]
    public String Password { get; set; }    
}