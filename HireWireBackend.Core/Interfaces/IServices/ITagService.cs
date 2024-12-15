using HireWireBackend.Core.Models;

namespace HireWireBackend.Core.Interfaces.IServices;

public interface ITagService
{
    Task<Tag> GetOrCreateTagByNameAsync(string name);
}