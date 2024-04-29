using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class ShoppingList
{
    public int Id { get; set; }

    public string User { get; set; }

    public virtual User UserNavigation { get; set; }
}
