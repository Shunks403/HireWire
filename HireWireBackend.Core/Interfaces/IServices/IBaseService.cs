namespace HireWireBackend.Core.Interfaces.IServices;

public interface IBaseService<T>
{
    
    Task<T> Update(T entity);

    Task<T> FindById(int id);

    Task Delete(int id);

    IEnumerable<T> GetAll();
}