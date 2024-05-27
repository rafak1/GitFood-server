using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class RecipesComment
{
    public string User { get; set; }

    public int? Recipe { get; set; }

    public int Id { get; set; }

    public string Message { get; set; }

    public int? Likes { get; set; }

    public DateTime? Date { get; set; }

    public virtual Recipe RecipeNavigation { get; set; }

    public virtual User UserNavigation { get; set; }
}
