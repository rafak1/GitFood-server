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

    public virtual ICollection<RecipiesIngredient> RecipiesIngredients { get; set; } = new List<RecipiesIngredient>();

    public virtual ICollection<FoodCategory> Categories { get; set; } = new List<FoodCategory>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
