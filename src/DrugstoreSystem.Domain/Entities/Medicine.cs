using DrugstoreSystem.Domain.Enums;

namespace DrugstoreSystem.Domain.Entities;

public class Medicine
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? GenericName { get; private set; }
    public string? NameRu { get; private set; }
    public DosageForm? DosageForm { get; private set; }
    public int? CategoryId { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int? CreatedByPharmacyId { get; private set; }

    public Category? Category { get; private set; }
    public ICollection<MedicineSynonym> Synonyms { get; private set; } = new List<MedicineSynonym>();
    public ICollection<PharmacyMedicine> PharmacyMedicines { get; private set; } = new List<PharmacyMedicine>();

    private Medicine() { }

    public Medicine(string name, string? genericName, string? nameRu, DosageForm? dosageForm,
        int? categoryId, string? manufacturer, string? description, int? createdByPharmacyId)
    {
        Name = name;
        GenericName = genericName;
        NameRu = nameRu;
        DosageForm = dosageForm;
        CategoryId = categoryId;
        Manufacturer = manufacturer;
        Description = description;
        CreatedByPharmacyId = createdByPharmacyId;
        CreatedAt = DateTime.UtcNow;
    }
}
