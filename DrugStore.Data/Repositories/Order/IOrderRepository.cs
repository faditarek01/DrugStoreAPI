
namespace DrugStore.Data.Repositories.Order
{
    public interface IOrderRepository
    {
        Task<GetDrugVM> GetDrugById(int id);
        Task<List<GetDrugVM>> GetDrugs(int itemCount = 10, int pageNumber = 1);
        Task<int> InsertSubOrders(string pharmacyId, List<SubOrderVM> suborders);
    }
}