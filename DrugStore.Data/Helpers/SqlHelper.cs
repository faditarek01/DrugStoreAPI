

namespace DrugStore.Data.Helpers
{
	public class SqlHelper
	{
		private readonly string _connectionString;
		private readonly SqlConnection _conn;

        public SqlHelper(IConfiguration configuration)
        {
			_connectionString = configuration.GetConnectionString("DefaultConnection"); 
			_conn = new SqlConnection(_connectionString);
		}

		public int ExecuteNonQuery(string connectionString, string commandText, params SqlParameter[] parameters)
		{
			try
			{
				_conn.ConnectionString = connectionString;
				if (_conn.State == ConnectionState.Closed)
					_conn.Open();
                SqlCommand cmd = new(commandText, _conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (parameters != null)
					cmd.Parameters.AddRange(parameters);

				var result = cmd.ExecuteNonQuery();
				return result;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message + "\n" + ex.StackTrace);
			}
			finally
			{
				if (_conn.State == ConnectionState.Open)
					_conn.Close();
			}
		}
	}
}
