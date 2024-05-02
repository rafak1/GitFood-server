using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsVerified { get; set; }

    public string Unit { get; set; }

    public virtual ICollection<AddCategoriesRequest> AddCategoriesRequests { get; set; } = new List<AddCategoriesRequest>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<RecipiesIngredient> RecipiesIngredients { get; set; } = new List<RecipiesIngredient>();

    public virtual ICollection<ShoppingListProduct> ShoppingListProducts { get; set; } = new List<ShoppingListProduct>();
}
