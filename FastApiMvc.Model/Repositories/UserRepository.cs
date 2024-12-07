using FastApiMvc.Model.Data;
using FastApiMvc.Model.Entities;
using FastApiMvc.Model.Interfaces;
using FastApiMvc.Model.Repositories;

namespace FastApiMvc.Model.Repositories;

public class UserRepository : BaseRepository<User>, IRepository<User>
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}
