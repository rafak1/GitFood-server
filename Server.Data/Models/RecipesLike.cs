using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class RecipesLike
{
    public string User { get; set; }

    public int? Recipe { get; set; }

    public virtual Recipe RecipeNavigation { get; set; }

    public virtual User UserNavigation { get; set; }
}
