
namespace DrugStore.Data.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public Status Status { get; set; }

        public List<SubOrderModel> SubOrders { get; set; } = new List<SubOrderModel>();

        public string? PharmacyId { get; set; }

        public bool IsArchived { get; set; }

        [ForeignKey(nameof(PharmacyId))]
        public virtual UserModel? User { get; set; }
    }

}
