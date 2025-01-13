
namespace DrugStore.Data.ViewModels
{
    public class UpdateQuantityInSubOrder
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int SubOrderId { get; set; }
        public int DrugId { get; set; }
        public int Quantity { get; set; }
    }
}

