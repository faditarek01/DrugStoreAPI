
namespace DrugStore.Data.Models
{
    public class DrugModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [StringLength(100, ErrorMessage =Errors.MaxLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = Errors.RequiredField)]
        [Range(1, int.MaxValue, ErrorMessage = Errors.MaxLength)]
        public int Quantity { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Range(0.01, double.MaxValue, ErrorMessage = Errors.MaxLength)]
        public decimal PricePerUnit { get; set; }

        public bool? Status { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [FutureDate(ErrorMessage = Errors.FutureDate)]
        public DateTime ExpiryDate { get; set; }

        public List<SubOrderModel> SubOrders { get; set; } = new List<SubOrderModel>(); 

    }
}
