using FastApiMvc.Model.Interfaces;

namespace FastApiMvc.Model.Entities;

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
