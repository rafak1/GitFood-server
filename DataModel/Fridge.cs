using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class Fridge
{
    public string UserLogin { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<FridgeProduct> FridgeProducts { get; set; } = new List<FridgeProduct>();

    public virtual User UserLoginNavigation { get; set; }
}
