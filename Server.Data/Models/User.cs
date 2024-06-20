using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class User
{
    public string Login { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string Verification { get; set; }

    public bool IsBanned { get; set; }

    public virtual ICollection<Fridge> Fridges { get; set; } = new List<Fridge>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<RecipesComment> RecipesComments { get; set; } = new List<RecipesComment>();

    public virtual ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();

    public virtual ICollection<User> Follows { get; set; } = new List<User>();

    public virtual ICollection<Fridge> FridgesNavigation { get; set; } = new List<Fridge>();

    public virtual ICollection<Recipe> RecipesNavigation { get; set; } = new List<Recipe>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
