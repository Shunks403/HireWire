namespace HireWireBackend.Core.Interfaces.IServices;

public interface IUserService: IBaseService<User>
{
    Task<User> Login(string email, string password);

    Task<User> Register(User user);
    
}