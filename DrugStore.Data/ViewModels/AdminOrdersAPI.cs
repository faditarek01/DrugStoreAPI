
namespace DrugStore.Data.ViewModels
{
    public class AdminOrdersAPI
    {
        public int OrderId { get; set; }
        public int SubOrderId { get; set; }
        public string CreatedAt { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice => Quantity * PricePerUnit;
        public string ExpiryDate { get; set; }
        public decimal TotaOrderPrice { get; set; }
        public string PharmacyName { get; set; }
        public string StatusOfOrderValue { get; set; }
    }
}
