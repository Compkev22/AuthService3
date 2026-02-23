using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;

    public class Role
    {
        [Key]
        [MaxLength(16)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        
        //Relaciones con UserRole
        public ICollection<UserRole> UserRoles { get; set; }
    }
