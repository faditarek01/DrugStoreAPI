
namespace DrugStore.Data.ViewModels
{
    public class CurrentSubOrderVM
    {
        public int OrderId { get; set; }
        public int SubOrderId { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? StatusOfDrug { get; set; }
        public string StatusOfDrugValue => !StatusOfDrug.HasValue ? "" : StatusOfDrug.Value ? "Available" : "Not Available"; 
        public decimal? PricePerUnit { get; set; }
        public decimal? TotalPrice => Quantity.HasValue && PricePerUnit.HasValue ? Quantity.Value * PricePerUnit.Value : null; 
        public Status StatusOfOrder { get; set; }
        public string StatusOfOrderValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? TotaOrderPrice { get; set; }
    }
}
