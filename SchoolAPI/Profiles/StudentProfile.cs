using AutoMapper;
using SchoolAPI.Models;

namespace SchoolAPI.Profiles
{
    public class StudentProfile : Profile
    {
        public static readonly string[] classesAsString = { "", "A", "B", "C", "D" }; // classes in database are stored as int 0-4, depends on the customer on how to display it
        public StudentProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.FullGrade, opt => opt.MapFrom(src => $"{src.Grade}{classesAsString[src.Class]}"));
            CreateProjection<Student, StudentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.FullGrade, opt => opt.MapFrom(src => $"{src.Grade}{classesAsString[src.Class]}"));

        }
    }
}
