// Models/Subscription.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Subscriptions")]
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string StripeSubscriptionId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string StripeCustomerId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string PriceId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // active, canceled, incomplete, etc.
        
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime? CanceledAt { get; set; }
        
        public string OrganizationId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}