using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Core.Services;

public class JobVacancyService : IJobVacancyService
{
    private readonly IRepository _repository;

    public JobVacancyService(IRepository repository)
    {
        _repository = repository;
    }


    public Task<JobVacancy> Update(JobVacancy entity)
    {
        return _repository.Update(entity);
    }

    public Task<JobVacancy> FindById(int id)
    {
        return _repository.GetById<JobVacancy>(id);
    }

    public Task Delete(int id)
    {
        var vacancy = FindById(id).Result;
        vacancy.IsDeleted = true;
        Update(vacancy);
        
        return Task.CompletedTask;
    }

    public Task<JobVacancy> Add(JobVacancy entity)
    {
        return _repository.Add(entity);
    }

    public IEnumerable<JobVacancy> GetJobVacanciesEmployer(int EmployerId)
    {
        var listJobVacancies = _repository.GetAll<JobVacancy>().Where(x => x.EmployerId == EmployerId && x.IsDeleted == false || x.IsDeleted == null ).ToList();
        return listJobVacancies;
    }

    public IEnumerable<JobVacancy> GetAll()
    {
        throw new NotImplementedException();
    }
    
    
    public async Task<(IEnumerable<JobVacancy> Jobs, int TotalPages)> GlobalSearch(string keywords, string location, int page, int pageSize)
    {
        // Получаем базовый запрос
        var query = _repository.GetAll<JobVacancy>()
            .Include(j => j.Employer) // Подгружаем данные работодателя
            .Include(j => j.JobVacancyTags) // Подгружаем теги
            .ThenInclude(jvt => jvt.Tag)
            .AsQueryable();

        // Применяем фильтрацию по ключевым словам
        if (!string.IsNullOrWhiteSpace(keywords))
        {
            query = query.Where(j => j.Title.Contains(keywords) || j.Description.Contains(keywords));
        }

        // Применяем фильтрацию по локации
        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(j => j.Location.Contains(location));
        }

        // Подсчитываем общее количество записей
        var totalJobs = await query.CountAsync();

        // Применяем пагинацию
        var jobs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Подсчитываем общее количество страниц
        var totalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);

        // Возвращаем результат
        return (jobs, totalPages);
    }
}