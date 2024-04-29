using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class Product
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Barcode { get; set; }

    public string User { get; set; }

    public int Id { get; set; }

    public int? Category { get; set; }

    public double? Quantity { get; set; }

    public virtual Category CategoryNavigation { get; set; }

    public virtual ICollection<FridgeProduct> FridgeProducts { get; set; } = new List<FridgeProduct>();
}
