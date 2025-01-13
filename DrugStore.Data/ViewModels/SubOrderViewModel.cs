
namespace DrugStore.Data.ViewModels
{
    public class SubOrderViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public int DrugId { get; set; }
        public bool IsAvailable { get; set; }


    }
}
