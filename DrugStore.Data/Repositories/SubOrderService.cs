
namespace DrugStore.Data.Repositories
{
    public class SubOrderService
    {
        private readonly IPharmacyRepository _pharmacyRepository;

        public SubOrderService(IPharmacyRepository pharmacyRepository)
        {
            _pharmacyRepository = pharmacyRepository;
        }

        public async Task<SubOrderViewModel> UpdateSubOrder(UpdateQuantityInSubOrder updatedQuantity)
        {
            var subOrderId = await _pharmacyRepository.UpdateQuantity(updatedQuantity);

            if (subOrderId == 0)
            {
                return new SubOrderViewModel(); 
            }

            return await _pharmacyRepository.GetSubOrder(updatedQuantity.OrderId, subOrderId);
        }

    }
}
