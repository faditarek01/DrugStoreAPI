
namespace DrugStore.Data.Repositories.Admin
{
    public interface IAdminRepository
    {
        byte[] ExportToExcel(bool IsArchived);
        Task<List<AdminOrdersAPI>> GetAdminOrdersAPI(bool IsArchived, int itemCount = 10, int pageNumber = 1);
        Task<List<UserModel>> GetPharmacies(string searchTerm, int itemCount = 10, int pageNumber = 1);
        Task<PharmacyInfo> GetPharmacy(string searchTerm);
        Task<PharmacyInfo> UpdatePharmacyName(UpdatePharmacyName model);
        Task<int> UpdateSubOrderQuantity(UpdateQuantityInSubOrder updatedQuantity);
    }
}
