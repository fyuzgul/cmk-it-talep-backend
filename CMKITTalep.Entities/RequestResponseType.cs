namespace CMKITTalep.Entities
{
    public class RequestResponseType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
