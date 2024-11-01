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
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.IsEmployer ? "Employer" : "Applicant"));
        CreateMap<Employer, EmployerDTO>().ReverseMap();
    }
    
}