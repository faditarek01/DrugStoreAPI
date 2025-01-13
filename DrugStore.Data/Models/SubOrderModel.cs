
namespace DrugStore.Data.Models
{
    public class SubOrderModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Range(1, int.MaxValue, ErrorMessage = Errors.MaxLength)]
        public int Quantity { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderModel Order { get; set; }

        public int DrugId { get; set; }

        [ForeignKey(nameof(DrugId))]
        public DrugModel Drug { get; set; }

        [AvailableQuantity(ErrorMessage = Errors.QuantitySize)]
        public bool IsAvailable { get; set; }
    }
}