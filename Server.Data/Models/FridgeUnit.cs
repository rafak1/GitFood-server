using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class FridgeUnit
{
    public int FridgeProductId { get; set; }

    public double Quantity { get; set; }

    public string Unit { get; set; }

    public virtual Fridge FridgeProduct { get; set; }
}
