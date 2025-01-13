
namespace DrugStore.Data.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<AuthModel> GetTokenAsync(string id);
    }
}