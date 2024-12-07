using FastApiMvc.Model.Dto.User;
using FastApiMvc.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastApiMvc.Api.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Success(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound<UserDto>();

            return Success(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            return Success(user);
        }
    }
}
