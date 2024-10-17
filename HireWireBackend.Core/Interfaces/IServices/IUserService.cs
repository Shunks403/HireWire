namespace HireWireBackend.Core.Interfaces.IServices;

public interface IUserService
{
    Task<User> Login(string email, string password);

    Task<User> Register(User user);

    Task<User> Update(User user);

    Task<User> FindById(int id);

    Task Delete(int id);

    IEnumerable<User> GetAll();

}