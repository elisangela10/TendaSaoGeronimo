namespace CasaDeAxe.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(UserRegisterRequest request);
    }


}
