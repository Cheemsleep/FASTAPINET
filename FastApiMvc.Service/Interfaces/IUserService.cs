using FastApiMvc.Common.Interfaces;
using FastApiMvc.Model.Dto.User;
using FastApiMvc.Model.Entities;

namespace FastApiMvc.Service.Interfaces;

public interface IUserService : IService<User>
{
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}
