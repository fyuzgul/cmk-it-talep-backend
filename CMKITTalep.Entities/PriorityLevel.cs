using System.ComponentModel.DataAnnotations;

namespace CMKITTalep.Entities
{
    public class PriorityLevel : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
