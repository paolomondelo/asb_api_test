namespace PetApiTests.Models;

public class StoreOrder
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public int Quantity { get; set; }
    public DateTime? ShipDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Complete { get; set; }
}

