namespace Server.Data.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();

    public virtual ICollection<Fridge> Fridges { get; set; } = new List<Fridge>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
