

namespace DrugStore.Data.Filters
{
    public class AvailableQuantityAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var subOrder = (SubOrderModel)validationContext.ObjectInstance;

            if (subOrder.Drug != null && subOrder.Quantity > 0)
            {
                if (subOrder.Drug.Quantity >= subOrder.Quantity)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }

            }

            return ValidationResult.Success;
        }
    }
}
