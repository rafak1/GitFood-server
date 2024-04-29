using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class RecipiesImage
{
    public string Name { get; set; }

    public string ImagePath { get; set; }

    public int Recipe { get; set; }

    public virtual Recipe RecipeNavigation { get; set; }
}
