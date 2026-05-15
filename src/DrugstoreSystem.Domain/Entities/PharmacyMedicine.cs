namespace DrugstoreSystem.Domain.Entities;

public class PharmacyMedicine
{
    public int Id { get; private set; }
    public int PharmacyId { get; private set; }
    public int MedicineId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Pharmacy Pharmacy { get; private set; } = default!;
    public Medicine Medicine { get; private set; } = default!;

    private PharmacyMedicine() { }

    public PharmacyMedicine(int pharmacyId, int medicineId, decimal price, int quantity)
    {
        PharmacyId = pharmacyId;
        MedicineId = medicineId;
        Price = price;
        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(decimal price, int quantity)
    {
        Price = price;
        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
