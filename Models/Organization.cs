using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Organizations")]
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string ClerkOrgId { get; set; } = string.Empty; // 👈 Este nombre debe coincidir
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        public int OwnerId { get; set; } // 👈 Este nombre debe coincidir

        [ForeignKey("OwnerId")]
        public User Owner { get; set; } = null!;
        
        // Nueva propiedad para la suscripción
        public bool IsActive { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}