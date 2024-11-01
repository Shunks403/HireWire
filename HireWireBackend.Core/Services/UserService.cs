using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;
using MessangerBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Core.Services;

public class UserService : IUserService
{
    private readonly IRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    
    public UserService(IRepository repository , IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }
    
    public Task<User> Login(string email, string password)
    {
        if (email == null || string.IsNullOrEmpty(email.Trim()) || 
            password == null || string.IsNullOrEmpty(password.Trim()))
        {
            throw new ArgumentNullException();
        }

        var user = _repository.GetAll<User>().Where(u => u.Email == email).SingleOrDefault();

        if (user != null && _passwordHasher.Verify(password, user.PasswordHash))
        {
            return Task.FromResult(user);
        }
        else
        {
            throw new InvalidOperationException("User not found or incorrect password.");
        }


    }

    public async Task<User> Register(User user)
    {
        var existingUser = await _repository.GetAll<User>()
            .AnyAsync(u => u.Email == user.Email);

        if (existingUser)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }
        user.CreatedAt = DateTime.Now;
        user.PasswordHash = _passwordHasher.Generate(user.PasswordHash);
        return await _repository.Add(user);
    }

    public Task<User> Update(User user)
    {
        return _repository.Update(user);
    }

    public Task<User> FindById(int id)
    {
        return _repository.GetById<User>(id);
    }

    public Task Delete(int id)
    {
        return _repository.Delete<User>(id);
    }

    public IEnumerable<User> GetAll()
    {
        return _repository.GetAll<User>().ToList();
    }
}