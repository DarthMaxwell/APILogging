using System.ComponentModel.DataAnnotations;

public class LoginDTO
{
    [Required]
    public required string Username { get; set; }
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}