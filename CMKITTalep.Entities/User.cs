namespace CMKITTalep.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int TypeId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public Department? Department { get; set; }
        public UserType? UserType { get; set; }
        public ICollection<MessageReadStatus> MessageReadStatuses { get; set; } = new List<MessageReadStatus>();
    }
}
