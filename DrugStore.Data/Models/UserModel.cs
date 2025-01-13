
namespace DrugStore.Data.Models
{
    public class UserModel 
    {
        public UserModel()
        {
            Orders = new List<OrderModel>();
        }

        [Required(ErrorMessage = Errors.RequiredField)]
        [StringLength(50, ErrorMessage = Errors.MaxLength)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = Errors.RequiredField)]
        [StringLength(50, ErrorMessage = Errors.MaxLength)]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = Errors.RequiredField)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^05[6,9]{1}[0-9]{7}$", ErrorMessage = Errors.PhonePattern)]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = Errors.RequiredField)]
        public int? AccountNum { get; set; }

        [MaxLength(50)]
        public string? PharmacyName { get; set; } 
        public List<OrderModel> Orders { get; set; }
        
    }
}