
namespace DrugStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly SubOrderService _subOrderService;

        public AdminController(IAdminRepository adminRepository, SubOrderService subOrderService)
        {
            _adminRepository = adminRepository;
            _subOrderService = subOrderService;
        }

        [HttpGet("AdminOrders")]
        public async Task<IActionResult> AdminOrders(bool isArchived, int itemCount = 10, int pageNumber = 1)
        {
            var orders = await _adminRepository.GetAdminOrdersAPI(isArchived, itemCount, pageNumber);

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found");
            }

            return Ok(orders);
        }


        [HttpPut("UpdateQuantityOfSubOrder")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityInSubOrder updatedQuantity)
        {
            if (updatedQuantity == null || updatedQuantity.Quantity <= 0)
            {
                return BadRequest("Invalid quantity");
            }

            var subOrder = await _subOrderService.UpdateSubOrder(updatedQuantity);

            if (subOrder == null)
            {
                return NotFound("Suborder update failed.");
            }

            return Ok(subOrder);
        }


        [HttpPut("UpdatePharmacyName")]
        public async Task<IActionResult> UpdatePharmacyName([FromBody] UpdatePharmacyName updatedQuantity)
        {
            if (updatedQuantity == null || string.IsNullOrEmpty(updatedQuantity.NewPharmacyName) || string.IsNullOrEmpty(updatedQuantity.PharmacyId))
            {
                return BadRequest("Invalid input");
            }

            var result = await _adminRepository.UpdatePharmacyName(updatedQuantity);

            if (result == null)
            {
                return NotFound("Pharmacy update failed.");
            }

            return Ok(result);
        }

        [HttpGet("AllPharmacies")]
        public async Task<IActionResult> GetPharmaciesInfo(string? pharName, int itemCount = 10, int pageNumber = 1)
        {
            var result = await _adminRepository.GetPharmacies(pharName);
            if (result is null)
            {
                return NotFound("please, try another name");
            }
            var pharmacies = new List<PharmacyVM>();
            foreach (var p in result)
            {
                PharmacyVM pharmacy = new PharmacyVM()
                {
                    AccountNum = p.AccountNum,
                    PharmacyName = p.PharmacyName
                };

                pharmacies.Add(pharmacy);
            }
            if (pharmacies.Count == 0)
            {
                return Ok("There's no Pharmacies");
            }
            return Ok(pharmacies);
        }

        [HttpGet("ExportOrdersToExcel")]
        public IActionResult ExportToExcel(bool isArchived)
        {
            try
            {
                var result = _adminRepository.ExportToExcel(isArchived);

                ExcelVM excel = new()
                {
                    content = result,
                    path = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    name = !isArchived ? "CurrentOrders.xlsx" : "CompletedOrders.xlsx"
                };

                return File(excel.content, excel.path, excel.name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

    }
}
