namespace DrugstoreSystem.Domain.Entities;

public class MedicineSynonym
{
    public int Id { get; private set; }
    public int MedicineId { get; private set; }
    public string Synonym { get; private set; } = default!;

    public Medicine Medicine { get; private set; } = default!;

    private MedicineSynonym() { }

    public MedicineSynonym(int medicineId, string synonym)
    {
        MedicineId = medicineId;
        Synonym = synonym;
    }
}
