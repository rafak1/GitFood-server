using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Author { get; set; }

    public string MarkdownPath { get; set; }

    public virtual User AuthorNavigation { get; set; }

    public virtual ICollection<RecipesComment> RecipesComments { get; set; } = new List<RecipesComment>();

    public virtual ICollection<RecipiesImage> RecipiesImages { get; set; } = new List<RecipiesImage>();
}
