
namespace DrugStore.Data.ViewModels
{
    public class UpdatePharmacyName
    {
        [Required]
        public string PharmacyId { get; set; }
        [Required]
        public string PharmacyName { get; set; }
        public string NewPharmacyName { get; set; }
    }
}
