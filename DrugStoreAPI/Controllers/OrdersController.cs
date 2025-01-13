namespace DrugStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "pharmacy")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("GetDrugs")]
        public async Task<IActionResult> GetDrugsAsync(int itemCount = 10, int pageNumber = 1)
        {
            var drugs = await _orderRepository.GetDrugs(itemCount, pageNumber);

            if (drugs?.Any() != true)
            {
                return Ok("There are no more drugs.");
            }

            return Ok(drugs);
        }

        [HttpGet("GetDrugById")]
        public async Task<IActionResult> GetDrugById(int id)
        {
            if (id <= 0)
            {
                return NotFound("Drug ID is not valid.");
            }

            var drug = await _orderRepository.GetDrugById(id);

            if (drug is null)
            {
                return BadRequest("There is no drug with this ID.");
            }

            return Ok(drug);
        }


        [HttpPost("AddNewOrder")]
        public async Task<IActionResult> InsertSubOrders(string pharmacyId, [FromBody] List<SubOrderVM>? suborders)
        {
            if (string.IsNullOrWhiteSpace(pharmacyId))
            {
                return BadRequest("Pharmacy ID is required.");
            }

            if (suborders == null || !suborders.Any())
            {
                return BadRequest("Suborders list is empty or missing.");
            }

            var result = await _orderRepository.InsertSubOrders(pharmacyId, suborders);

            if (result > 0)
            {
                return Ok($"Order created successfully with ID: {result}");
            }

            return StatusCode(500, "An error occurred while creating the order.");
        }

    }
}
