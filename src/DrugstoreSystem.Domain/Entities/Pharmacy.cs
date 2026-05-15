namespace DrugstoreSystem.Domain.Entities;

public class Pharmacy
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Phone { get; private set; }
    public string? WorkingHours { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<PharmacyMedicine> PharmacyMedicines { get; private set; } = new List<PharmacyMedicine>();

    private Pharmacy() { }

    public Pharmacy(string name, string address, double latitude, double longitude,
        string? phone, string? workingHours)
    {
        Name = name;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
        Phone = phone;
        WorkingHours = workingHours;
        IsActive = true;
        IsVerified = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void Verify() => IsVerified = true;

    public void Update(string name, string address, double latitude, double longitude,
        string? phone, string? workingHours)
    {
        Name = name;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
        Phone = phone;
        WorkingHours = workingHours;
    }
}
