namespace DrugstoreSystem.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;

    public ICollection<Medicine> Medicines { get; private set; } = new List<Medicine>();

    private Category() { }

    public Category(string name)
    {
        Name = name;
    }
}
