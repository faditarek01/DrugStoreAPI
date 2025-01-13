
namespace DrugStore.Data.Repositories.Pharmacy
{
    public interface IPharmacyRepository
    {
        Task<OrderViewModel> ArchiveOrder(int orderId);
        Task<int> DeleteRow(DeleteRowInCurrentOrder row);
        Task<List<CurrentSubOrderVM>> GetPharmacyOrders(string pharId, bool IsArchived, int itemCount = 10, int pageNumber = 1);
        Task<SubOrderViewModel> GetSubOrder(int orderId, int subId);
        Task<UserViewModel> GetUser(string Id);
        Task<int> UpdateQuantity(UpdateQuantityInSubOrder updatedQuantity);
    }
}