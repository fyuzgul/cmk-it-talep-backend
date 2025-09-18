using System.ComponentModel.DataAnnotations;

namespace CMKITTalep.Entities
{
    public class RequestCC : BaseEntity
    {
        public int RequestId { get; set; } // Foreign Key to Request
        public int UserId { get; set; } // Foreign Key to User (CC'deki kullanıcı)
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public Request? Request { get; set; }
        public User? User { get; set; }
    }
}
