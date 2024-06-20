using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class FoodCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Recipe> Reciepes { get; set; } = new List<Recipe>();
}
