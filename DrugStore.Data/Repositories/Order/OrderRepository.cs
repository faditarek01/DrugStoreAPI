
namespace DrugStore.Data.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly SqlConnection _conn;
        private readonly IPharmacyRepository _pharmacyRepository;

        public OrderRepository(IConfiguration configuration, IPharmacyRepository pharmacyRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _conn = new SqlConnection(_connectionString);
            _pharmacyRepository = pharmacyRepository;
        }

        public async Task<List<GetDrugVM>> GetDrugs(int itemCount = 10, int pageNumber = 1)
        {
            SqlDataAdapter da = new("GetDrugs", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@ItemCount", itemCount);
            da.SelectCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
            DataTable dt = new();
            da.Fill(dt);
            List<GetDrugVM> Drugs = new();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetDrugVM drug = new()
                    {
                        Name = dt.Rows[i]["Name"].ToString(),
                        Quantity = Convert.ToInt32(dt.Rows[i]["Quantity"]),
                        PricePerUnit = Convert.ToDecimal(dt.Rows[i]["PricePerUnit"]),
                        Status = Convert.ToBoolean(dt.Rows[i]["Status"]),
                        ExpiryDate = Convert.ToDateTime(dt.Rows[i]["ExpiryDate"]).ToShortDateString(),
                    };
                    Drugs.Add(drug);
                }
            }
            if (Drugs.Count > 0)
            {
                return Drugs;
            }
            return [];
        }

        public async Task<GetDrugVM> GetDrugById(int id)
        {
            SqlDataAdapter da = new("GetDrugs", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            da.SelectCommand.Parameters.AddWithValue("@DrugId", id);
            DataTable dt = new();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                GetDrugVM drug = new()
                {
                    Name = dt.Rows[0]["Name"].ToString(),
                    Quantity = Convert.ToInt32(dt.Rows[0]["Quantity"]),
                    PricePerUnit = Convert.ToDecimal(dt.Rows[0]["PricePerUnit"]),
                    Status = Convert.ToBoolean(dt.Rows[0]["Status"]),
                    ExpiryDate = Convert.ToDateTime(dt.Rows[0]["ExpiryDate"]).ToShortDateString(),
                };

                if (drug is null)
                {
                    return new GetDrugVM();
                }
                return drug;
            }
            return new GetDrugVM();
        }

        public async Task<int> InsertSubOrders(string pharmacyId, List<SubOrderVM> suborders)
        {
            
            var pharmacy = await _pharmacyRepository.GetUser(pharmacyId);
            if (pharmacy == null)
            {
                return 0;
            }

            var subOrdersTable = new DataTable();
            subOrdersTable.Columns.Add("DrugId", typeof(int));
            subOrdersTable.Columns.Add("Quantity", typeof(int));

            foreach (var sub in suborders)
            {
                subOrdersTable.Rows.Add(sub.DrugId, sub.Quantity);
            }

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("InsertSubOrders", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PharmacyId", pharmacyId);
            command.Parameters.AddWithValue("@SubOrders", subOrdersTable);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("OrderId")); 
            }

            return 0; 
        }



    }
}
