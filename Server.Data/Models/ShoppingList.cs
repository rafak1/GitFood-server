using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class ShoppingList
{    
    public int Id { get; set; }

    public string User { get; set; }

    public string Name { get; set; }

    public virtual ICollection<ShoppingListProduct> ShoppingListProducts { get; set; } = new List<ShoppingListProduct>();

    public virtual User UserNavigation { get; set; }
}
