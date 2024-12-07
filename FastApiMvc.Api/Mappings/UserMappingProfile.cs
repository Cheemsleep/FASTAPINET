using AutoMapper;
using FastApiMvc.Model.Dto.User;
using FastApiMvc.Model.Entities;

namespace FastApiMvc.Api.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
        }
    }
}
