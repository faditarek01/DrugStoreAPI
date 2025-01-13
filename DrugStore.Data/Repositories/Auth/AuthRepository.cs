
namespace DrugStore.Data.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;
        private readonly JWT _jwt;
        private readonly IPharmacyRepository _pharmacyRepository;

        public AuthRepository(IConfiguration configuration, IOptions<JWT> jwt, IPharmacyRepository pharmacyRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwt = jwt.Value;
            _pharmacyRepository = pharmacyRepository;
        }

        public async Task<AuthModel> GetTokenAsync(string id)
        {
            var authModel = new AuthModel();

            var user = await _pharmacyRepository.GetUser(id);

            if (user is null)
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await GetUserRoles(id);

            authModel.Message = "Success";
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo.ToShortDateString();
            authModel.Roles = rolesList.Select(c => c.Value).ToList(); ;

            return authModel;
        }





        private async Task<JwtSecurityToken> CreateJwtToken(UserViewModel user)
        {
            var roleClaims = await GetUserRoles(user.Id);

            var claims = new List<Claim>
            {
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("UserId", user.Id),
            new("UserUserName", user.UserName),
            new("UserEmail", user.Email),
            };

            claims.AddRange(roleClaims);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials
            );
        }


        private async Task<IEnumerable<Claim>> GetUserRoles(string userId)
        {
            var claims = new List<Claim>();

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await using var command = new SqlCommand("GetUserRoles", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", userId));

                // Open the connection
                await connection.OpenAsync();

                // Execute the reader
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    claims.Add(new Claim("roles", reader["Role"].ToString()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return claims;
        }
    }
}
