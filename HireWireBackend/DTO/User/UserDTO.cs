using System.ComponentModel.DataAnnotations;

namespace HireWireBackend.DTO;

public class UserDTO
{
    [Required]
    public String FirstName { get; set; }
    
    [Required]
    public String LastName { get; set; }
    
    [Required]
    public String Email { get; set; }
    
    [Required]
    public String Role { get; set; }
    
    [Required]
    public String Password { get; set; }       

   

}