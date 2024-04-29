using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class RecipiesIngredient
{
    public int Reciepie { get; set; }

    public double? Quantity { get; set; }

    public int? Category { get; set; }

    public virtual Category CategoryNavigation { get; set; }

    public virtual Recipe ReciepieNavigation { get; set; }
}
