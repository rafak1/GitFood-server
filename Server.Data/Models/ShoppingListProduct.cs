using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class ShoppingListProduct
{
    public int ShoppingListId { get; set; }

    public int Category { get; set; }

    public double? Quantity { get; set; }

    public virtual Category CategoryNavigation { get; set; }

    public virtual ShoppingList ShoppingList { get; set; }
}
