using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class FridgeProduct
{
    public int ProductId { get; set; }

    public int FridgeId { get; set; }

    public double? Ammount { get; set; }

    public virtual Fridge Fridge { get; set; }

    public virtual Product Product { get; set; }
}
