namespace FastApiMvc.Model.Interfaces;

public interface IEntity
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
}
