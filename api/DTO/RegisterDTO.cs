using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required]
    public required string Username { get; set; }
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
    [Required]
    [MinLength(6)]
    public required string ConfirmPassword { get; set; }
}