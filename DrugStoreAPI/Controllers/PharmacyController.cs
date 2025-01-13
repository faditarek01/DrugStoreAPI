
namespace DrugStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "pharmacy")]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly SubOrderService _subOrderService;

        public PharmacyController(IPharmacyRepository pharmacyRepository, SubOrderService subOrderService)
        {
            _pharmacyRepository = pharmacyRepository;
            _subOrderService = subOrderService;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetLoggedInPharmacy(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _pharmacyRepository.GetUser(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpGet("PharmacyOrders")]

        public async Task<IActionResult> PharmacyOrders(string Id, bool IsArchived, int itemCount = 10, int pageNumber = 1)
        {
            var result = await _pharmacyRepository.GetPharmacyOrders(Id, IsArchived,itemCount,pageNumber);
            return Ok(result);
        }

        [HttpPut("ArchiveOrder")]
        public async Task<IActionResult> ArchiveOrder(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            var result = await _pharmacyRepository.ArchiveOrder(orderId);

            if (result != null)
            {
                return Ok($"Order {result.Id} archived successfully.");
            }

            return NotFound("Order not found or could not be archived.");
        }


        [HttpPut("UpdateQuantityOfSubOrder")]
        public async Task<IActionResult> UpdateQuantityInSubOrder([FromBody] UpdateQuantityInSubOrder updatedQuantity)
        {
            if (updatedQuantity is null)
            {
                return BadRequest("model cannot be null.");
            }

            if (updatedQuantity.Quantity <= 0)
            {
                return BadRequest("Quantity must be a positive number.");
            }

            var subOrder = await _subOrderService.UpdateSubOrder(updatedQuantity);

            if (subOrder != null)
            {
                return Ok(subOrder);
            }

            return BadRequest("SubOrder could not be updated. Please check the provided data.");
        }


        [HttpDelete("DeleteSubOrder")]
        public async Task<IActionResult> DeleteSubOrderAsync([FromBody] DeleteRowInCurrentOrder row)
        {
            if (row is null)
            {
                return BadRequest("model cannot be null.");
            }

            var result = await _pharmacyRepository.DeleteRow(row);

            if (result > 0)
            {
                return Ok("SubOrder deleted successfully.");
            }

            return BadRequest("Failed to delete the SubOrder.");
        }

    }
}