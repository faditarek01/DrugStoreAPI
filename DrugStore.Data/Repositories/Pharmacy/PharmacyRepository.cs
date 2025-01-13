
using DrugStore.Data.ViewModels;

namespace DrugStore.Data.Repositories.Pharmacy
{
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly string _connectionString;
        private readonly SqlConnection _conn;
        private readonly SqlHelper _sqlHelper;


        public PharmacyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _conn = new SqlConnection(_connectionString);
            _sqlHelper = new SqlHelper(configuration);
        }

        public async Task<UserViewModel?> GetUser(string id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand("GetUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = new UserViewModel
                {
                    Id = reader["Id"].ToString(),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    RoleName = reader["RoleName"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                };

                if (user.RoleName == Roles.pharmacy.ToString())
                {
                    user.AccountNumber = Convert.ToInt32(reader["AccountNum"]);
                    user.PharmacyName = reader["PharmacyName"].ToString();
                }

                return user;
            }

            return new UserViewModel();
        }


        public async Task<List<CurrentSubOrderVM>> GetPharmacyOrders(string pharId, bool isArchived, int itemCount = 10, int pageNumber = 1)
        {
            var user = await GetUser(pharId);  
            if (user is null)
            {
                return []; 
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("GetPharmacyOrders", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PharId", pharId);
            command.Parameters.AddWithValue("@IsArchived", isArchived);
            command.Parameters.AddWithValue("@ItemCount", itemCount);
            command.Parameters.AddWithValue("@PageNumber", pageNumber);

            var orders = new List<CurrentSubOrderVM>();

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var order = new CurrentSubOrderVM
                    {
                        OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                        SubOrderId = reader.GetInt32(reader.GetOrdinal("SubOrderId")),
                        DrugId = reader.GetInt32(reader.GetOrdinal("DrugId")),
                        StatusOfDrug = reader.GetBoolean(reader.GetOrdinal("DrugStatus")),
                        DrugName = reader["DrugName"].ToString(),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                        PricePerUnit = reader.GetDecimal(reader.GetOrdinal("PricePerUnit")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        StatusOfOrderValue = reader["OrderStatus"].ToString(),
                        TotaOrderPrice = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                    };

                    orders.Add(order);
                }
            }

            return orders;
        }


        public async Task<OrderViewModel> ArchiveOrder(int orderId)
        {
            SqlParameter param_orderId = new("@OrderId", orderId);

            _sqlHelper.ExecuteNonQuery(_connectionString, "ArchiveOrder", param_orderId);

            var order = await GetOrder(orderId);

            return order;
        }

        private async Task<OrderViewModel> GetOrder(int orderId)
        {
            using SqlDataAdapter da = new("GetOrderById", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@OrderId", orderId);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                var Order = new OrderViewModel
                {
                    Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    CreatedAt = Convert.ToDateTime(dt.Rows[0]["CreatedAt"]).ToShortDateString(),
                    Status = Enum.TryParse<Status>(dt.Rows[0]["Status"].ToString(), out var status) ? status : Status.Pending,
                    PharmacyId = dt.Rows[0]["PharmacyId"].ToString(),
                    IsArchived = Convert.ToBoolean(dt.Rows[0]["IsArchived"])
                };

                return Order;
            }
            return null;
        }

        public async Task<int> UpdateQuantity(UpdateQuantityInSubOrder updatedQuantity)
        {
            var subOrder = await GetSubOrder(updatedQuantity.OrderId, updatedQuantity.SubOrderId);

            if (subOrder == null)
            {
                return 0; 
            }

            await UpdateSubOrder(subOrder.Id, updatedQuantity.Quantity, updatedQuantity.DrugId);
            return subOrder.Id;
        }


        public async Task<SubOrderViewModel> GetSubOrder(int orderId, int subId)
        {
            using var da = new SqlDataAdapter("GetSubOrder", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@OrderId", orderId);
            da.SelectCommand.Parameters.AddWithValue("@SubId", subId);

            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return new SubOrderViewModel
                {
                    Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    OrderId = Convert.ToInt32(dt.Rows[0]["OrderId"]),
                    Quantity = Convert.ToInt32(dt.Rows[0]["Quantity"]),
                    DrugId = Convert.ToInt32(dt.Rows[0]["DrugId"]),
                    IsAvailable = Convert.ToBoolean(dt.Rows[0]["IsAvailable"])
                };
            }

            return new SubOrderViewModel();
        }


        private async Task UpdateSubOrder(int subOrderId, int quantity, int drugId)
        {
            var param_subOrderId = new SqlParameter("@SubOrderId", subOrderId);
            var param_quantity = new SqlParameter("@Quantity", quantity);
            var param_drugId = new SqlParameter("@DrugId", drugId);

             _sqlHelper.ExecuteNonQuery(_connectionString, "UpdateSubOrder", param_subOrderId, param_quantity, param_drugId);
        }

        public async Task<int> DeleteRow(DeleteRowInCurrentOrder row)
        {
            var subOrder = await GetSubOrder(row.OrderId, row.SubId);
            var subOrderId = subOrder.Id;
            if (subOrderId != 0)
            {
                await DeleteSubOrder(row.SubId, row.OrderId);
                return subOrderId;
            }
            return 0;
        }

        private async Task DeleteSubOrder(int suborderId, int orderId)
        {

            var param_orderId = new SqlParameter("@OrderId", orderId);
            var param_subOrderId = new SqlParameter("@SubId", suborderId);

            _sqlHelper.ExecuteNonQuery(_connectionString, "DeleteSubOrder", param_orderId, param_subOrderId);
        }
    }
}
