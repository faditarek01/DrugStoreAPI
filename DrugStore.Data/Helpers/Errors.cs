using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrugStore.Data.Helpers
{
    public class Errors
    {
        public const string RequiredField = "Required field";
        public const string MaxLength = "Length cannot be more than {1} characters";
        public const string FutureDate = "Expiry date must be in the future";
        public const string QuantitySize = "Not enough quantity available for the selected drug";
        public const string PhonePattern = "Not enough quantity available for the selected drug";

    }
}
