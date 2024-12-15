using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.Core.Models;
using LibraryManegerBackend.Core.Interfaces;

namespace HireWireBackend.Core.Services;

public class TagService : ITagService
{
    private readonly IRepository _repository;

    public TagService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Tag> GetOrCreateTagByNameAsync(string name)
    {
        // Ищем тег в базе данных через репозиторий
        var tag = _repository.GetAll<Tag>().FirstOrDefault(t => t.Name == name);
        
        // Если тег не найден, создаем его
        if (tag == null)
        {
            tag = new Tag { Name = name };
            await _repository.Add(tag);
        }

        return tag;
    }
}