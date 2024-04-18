using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class Fridge
{
    public int ProductId { get; set; }

    public string UserLogin { get; set; }

    public int Id { get; set; }

    public virtual ICollection<FridgeUnit> FridgeUnits { get; set; } = new List<FridgeUnit>();

    public virtual Product Product { get; set; }

    public virtual User UserLoginNavigation { get; set; }
}
