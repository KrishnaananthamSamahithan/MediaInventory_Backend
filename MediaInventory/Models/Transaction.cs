using MediaInventoryMVC.Models;

public class Transaction
{
    public int Id { get; set; }
    public int InventoryId { get; set; }
    public string TransactionType { get; set; } // "Borrow", "Return"
    public string UserName { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Reason { get; set; } // New field for the borrow reason

    // Navigation property
    public Inventory? Inventory { get; set; }
}
