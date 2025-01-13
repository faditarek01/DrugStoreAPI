
namespace DrugStore.Data.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;
        private readonly SqlConnection _conn;
        private readonly SqlHelper _sqlHelper;
        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _conn = new SqlConnection(_connectionString);
            _sqlHelper = new SqlHelper(configuration);
        }

        public async Task<List<AdminOrdersAPI>> GetAdminOrdersAPI(bool isArchived, int itemCount = 10, int pageNumber = 1)
        {
            using SqlDataAdapter da = new("GetAdminOrders", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@IsArchived", isArchived);
            da.SelectCommand.Parameters.AddWithValue("@ItemCount", itemCount);
            da.SelectCommand.Parameters.AddWithValue("@PageNumber", pageNumber);

            DataTable dt = new();

            da.Fill(dt);

            var ordersList = GenerateOrdersListFromDataTable(dt);

            return ordersList.Any() ? ordersList : null;
        }

        private static List<AdminOrdersAPI> GenerateOrdersListFromDataTable(DataTable dt)
        {
            List<AdminOrdersAPI> OrdersLst = new();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AdminOrdersAPI order = new()
                    {
                        PharmacyName = dt.Rows[i]["PharmacyName"].ToString(),
                        OrderId = Convert.ToInt32(dt.Rows[i]["OrderId"]),
                        SubOrderId = Convert.ToInt32(dt.Rows[i]["SubOrderId"]),
                        DrugId = Convert.ToInt32(dt.Rows[i]["DrugId"]),
                        DrugName = dt.Rows[i]["DrugName"].ToString(),
                        Quantity = Convert.ToInt32(dt.Rows[i]["Quantity"]),
                        ExpiryDate = Convert.ToDateTime(dt.Rows[i]["ExpiryDate"]).ToShortDateString(),
                        PricePerUnit = Convert.ToDecimal(dt.Rows[i]["PricePerUnit"]),
                        CreatedAt = Convert.ToDateTime(dt.Rows[i]["CreatedAt"]).ToShortDateString(),
                        StatusOfOrderValue = dt.Rows[i]["OrderStatus"].ToString(),
                        TotaOrderPrice = Convert.ToDecimal(dt.Rows[i]["TotalAmount"])
                    };
                    OrdersLst.Add(order);
                }
            }
            return OrdersLst;
        }



        public async Task<int> UpdateSubOrderQuantity(UpdateQuantityInSubOrder updatedQuantity)
        {
            if (updatedQuantity is null)
            {
                return 0;
            }

            var subOrderId = await GetSubOrderId(updatedQuantity.OrderId, updatedQuantity.SubOrderId);

            if (subOrderId != 0)
            {
                await UpdateSubOrder(subOrderId, updatedQuantity.Quantity, updatedQuantity.DrugId);
                return subOrderId;
            }

            return 0;
        }

        private async Task<int> GetSubOrderId(int orderId, int subId)
        {
            var command = new SqlCommand("GetSubOrder", _conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@SubId", subId);

            _conn.Open();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (int)reader["Id"];
            }

            return 0;
        }

        private async Task UpdateSubOrder(int subOrderId, int quantity, int drugId)
        {
            SqlParameter param_subOrderId = new("@SubOrderId", subOrderId);
            SqlParameter param_quantity = new("@Quantity", quantity);
            SqlParameter param_drugId = new("@DrugId", drugId);
            _sqlHelper.ExecuteNonQuery(_connectionString, "UpdateSubOrder", param_subOrderId, param_quantity, param_drugId);
        }

        public async Task<List<DrugModel>> GetDrugs()
        {
            SqlDataAdapter da = new("GetDrugs", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            System.Data.DataTable dt = new();
            da.Fill(dt);
            List<DrugModel> DrugsLst = new();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DrugModel drug = new()
                    {
                        Name = dt.Rows[i]["Name"].ToString(),
                        Quantity = Convert.ToInt32(dt.Rows[i]["Quantity"]),
                        PricePerUnit = Convert.ToDecimal(dt.Rows[i]["PricePerUnit"]),
                        Status = Convert.ToBoolean(dt.Rows[i]["Status"]),
                        ExpiryDate = Convert.ToDateTime(dt.Rows[i]["ExpiryDate"]),
                    };
                    DrugsLst.Add(drug);
                }
            }
            return DrugsLst;
        }

        public async Task<PharmacyInfo> UpdatePharmacyName(UpdatePharmacyName model)
        {
            if (string.IsNullOrEmpty(model.PharmacyName) || string.IsNullOrEmpty(model.NewPharmacyName))
            {
                return new PharmacyInfo(); 
            }

            // Get pharmacies matching the name
            var pharmacies = await GetPharmacies(model.PharmacyName);
            var pharmacy = pharmacies?.FirstOrDefault();

            if (pharmacy == null)
            {
                return new PharmacyInfo();
            }

            SqlParameter param_pharmacyId = new("@UserId", model.PharmacyId);
            SqlParameter param_newPharmacyName = new("@NewPharmacyName", model.NewPharmacyName);

            try
            {
                _sqlHelper.ExecuteNonQuery(_connectionString, "UpdatePharmacyName", param_pharmacyId, param_newPharmacyName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating pharmacy name: " + ex.Message, ex);
            }

            return await GetPharmacy(model.PharmacyId);
        }



        public async Task<List<UserModel>> GetPharmacies(string searchTerm, int itemCount = 10, int pageNumber = 1)
        {
            SqlDataAdapter da = new("GetPharmacies", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@PharName", searchTerm);
            da.SelectCommand.Parameters.AddWithValue("@ItemCount", itemCount);
            da.SelectCommand.Parameters.AddWithValue("@PageNumber", pageNumber);

            DataTable dt = new();
            da.Fill(dt);

            var pharmacies = new List<UserModel>();
            foreach (DataRow row in dt.Rows)
            {
                pharmacies.Add(new UserModel
                {
                    AccountNum = Convert.ToInt32(row["AccountNum"]),
                    PharmacyName = row["PharmacyName"].ToString()
                });
            }

            return pharmacies.Count != 0 ? pharmacies : [];
        }


        public async Task<PharmacyInfo> GetPharmacy(string pharmacyId)
        {
            SqlDataAdapter da = new("GetPharmacy", _conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@PharmacyId", pharmacyId);

            DataTable dt = new();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return new PharmacyInfo
                {
                    UserName = dt.Rows[0]["UserName"].ToString(),
                    Email = dt.Rows[0]["Email"].ToString(),
                    RoleName = dt.Rows[0]["Role Name"].ToString()
                };
            }

            return new PharmacyInfo();
        }



        public byte[] ExportToExcel(bool IsArchived)
        {
            try
            {
                using var da = new SqlDataAdapter("GetAdminOrders", _conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@IsArchived", IsArchived);

                DataTable dt = new();
                da.Fill(dt);

                List<AdminOrdersAPI> OrdersLst = GenerateOrdersListFromDataTable(dt);

                using var workbook = new XLWorkbook();
                string worksheetTitle = IsArchived ? "CompletedOrders" : "CurrentOrders";
                var worksheet = workbook.Worksheets.Add(worksheetTitle);

                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Created At";
                worksheet.Cell(currentRow, 2).Value = "Drug Name";
                worksheet.Cell(currentRow, 3).Value = "Quantity";
                worksheet.Cell(currentRow, 4).Value = "Price Per Unit";
                worksheet.Cell(currentRow, 5).Value = "Total Price";
                worksheet.Cell(currentRow, 6).Value = "Expiry Date";

                worksheet.Row(currentRow).Style.Font.Bold = true;
                worksheet.Row(currentRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var cr in OrdersLst)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = cr.CreatedAt;
                    worksheet.Cell(currentRow, 2).Value = cr.DrugName;
                    worksheet.Cell(currentRow, 3).Value = cr.Quantity;
                    worksheet.Cell(currentRow, 4).Value = cr.PricePerUnit;
                    worksheet.Cell(currentRow, 5).Value = cr.TotalPrice;
                    worksheet.Cell(currentRow, 6).Value = cr.ExpiryDate;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while exporting to Excel: " + ex.Message, ex);
            }
        }


    }
}
