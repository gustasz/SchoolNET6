using AutoMapper;
using SchoolAPI.Models;

namespace SchoolAPI
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<Subject, SubjectDto>();
        }
    }
}
