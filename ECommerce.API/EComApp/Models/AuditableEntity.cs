namespace ECommerceAPI.Models
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "System";
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = "System";
    }
}
