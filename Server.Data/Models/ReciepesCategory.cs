using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class ReciepesCategory
{
    public int? Reciepe { get; set; }

    public int? Category { get; set; }

    public virtual FoodCategory CategoryNavigation { get; set; }

    public virtual Recipe ReciepeNavigation { get; set; }
}
