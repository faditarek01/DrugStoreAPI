using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DrugStore.Data.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public Status Status { get; set; }
        public string? PharmacyId { get; set; }
        public bool IsArchived { get; set; }
    }
}
