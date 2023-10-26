using Management.Domain.Common;

namespace Management.Domain.Entities;

public class Product: BaseEntity
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<SavedFile> Images { get; set; } = new List<SavedFile>();
}
