using AutoMapper;
using FastApiMvc.Common.Exceptions;
using FastApiMvc.Model.Dto.User;
using FastApiMvc.Model.Entities;
using FastApiMvc.Model.Interfaces;
using FastApiMvc.Service.Interfaces;

namespace FastApiMvc.Service.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IMapper _mapper;

        public UserService(IRepository<User> repository, IMapper mapper) 
            : base(repository)
        {
            _mapper = mapper;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (!await IsEmailUniqueAsync(createUserDto.Email))
            {
                throw new BusinessException("Email already exists");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
            };

            await _repository.AddAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var users = await _repository.FindAsync(u => u.Email == email);
            var user = users.FirstOrDefault();
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var users = await _repository.FindAsync(u => u.Email == email);
            return !users.Any();
        }
    }
}
