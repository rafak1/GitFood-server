using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class User
{
    public string Login { get; set; }

    public string Password { get; set; }

    public virtual ICollection<Fridge> Fridges { get; set; } = new List<Fridge>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<RecipesComment> RecipesComments { get; set; } = new List<RecipesComment>();

    public virtual ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
}
