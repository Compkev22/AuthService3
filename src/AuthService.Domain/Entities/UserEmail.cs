using System.ComponentModel.DataAnnotations;
namespace AuthService.Domain.Entities;

public class UserEmail
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
    [MaxLength(16)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public bool EmailVerified { get; set; }

    [MaxLength(256)]
    public string? EmailVerificationToken { get; set; } = string.Empty;

    public DateTime? EmailVerificationTokenExpiration { get; set; }

    // Propiedad de navegación hacia la entidad User
    public virtual User User { get; set; } = default!;
}