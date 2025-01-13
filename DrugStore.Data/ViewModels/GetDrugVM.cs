
namespace DrugStore.Data.ViewModels
{
    public class GetDrugVM
    {
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public bool Status { get; set; }
        public string ExpiryDate { get; set; }
    }
}
