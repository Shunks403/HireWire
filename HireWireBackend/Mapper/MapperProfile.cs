using AutoMapper;
using HireWireBackend.DTO;


namespace HireWireBackend.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
            .ReverseMap()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        CreateMap<UserRegistrationDto, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.IsEmployer ? "Employer" : "Applicant"))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        CreateMap<Employer, EmployerDTO>().ReverseMap();
        CreateMap<JobVacancy, JobVacancyDTO>().ReverseMap();
        CreateMap<Applicant, ApplicantDTO>().ReverseMap();
        CreateMap<JobApplication, JobApplicationDTO>().ReverseMap();
        
        
        
    }
    
}