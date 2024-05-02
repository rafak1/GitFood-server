using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class RecipeChild
{
    public int? Recipe { get; set; }

    public int? Child { get; set; }

    public double? Multiplier { get; set; }
}
