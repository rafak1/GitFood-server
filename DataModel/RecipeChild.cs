using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class RecipeChild
{
    public int? Recipe { get; set; }

    public int? Child { get; set; }

    public double? Multiplier { get; set; }
}
