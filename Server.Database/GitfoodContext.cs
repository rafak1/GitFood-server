using Server.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Server.Database;

public partial class GitfoodContext : DbContext
{
    private readonly IConfiguration _configuration;
    public GitfoodContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public GitfoodContext(DbContextOptions<GitfoodContext> options, IConfiguration configuration)
        : base(options)
    {
         _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public virtual DbSet<AddCategoriesRequest> AddCategoriesRequests { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<FoodCategory> FoodCategories { get; set; }

    public virtual DbSet<Fridge> Fridges { get; set; }

    public virtual DbSet<FridgeProduct> FridgeProducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeChild> RecipeChildren { get; set; }

    public virtual DbSet<RecipesComment> RecipesComments { get; set; }

    public virtual DbSet<RecipiesImage> RecipiesImages { get; set; }

    public virtual DbSet<RecipiesIngredient> RecipiesIngredients { get; set; }

    public virtual DbSet<ShoppingList> ShoppingLists { get; set; }

    public virtual DbSet<ShoppingListProduct> ShoppingListProducts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        if(bool.TryParse(_configuration.GetSection("LocalConfig").GetSection("UseLocalDb").Value, out var useLocalDb) && useLocalDb){
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("LocalDb"));
        }
        else
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddCategoriesRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("add_categories_request_pkey");

            entity.ToTable("add_categories_request");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datetime).HasColumnName("datetime");
            entity.Property(e => e.Request).HasColumnName("request");
            entity.Property(e => e.User)
                .HasColumnType("character varying")
                .HasColumnName("user");

            entity.HasOne(d => d.RequestNavigation).WithMany(p => p.AddCategoriesRequests)
                .HasForeignKey(d => d.Request)
                .HasConstraintName("add_categories_request_request_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasColumnType("character varying")
                .HasColumnName("unit");
        });

        modelBuilder.Entity<FoodCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("food_categories_pkey");

            entity.ToTable("food_categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Fridge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fridge_pkey");

            entity.ToTable("fridge");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.UserLogin)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("user_login");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.Fridges)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fridge_user_login_fkey");

            entity.HasMany(d => d.Users).WithMany(p => p.FridgesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "FridgeShare",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fridge_shares_user_fkey"),
                    l => l.HasOne<Fridge>().WithMany()
                        .HasForeignKey("FridgeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fridge_shares_fridge_id_fkey"),
                    j =>
                    {
                        j.HasKey("FridgeId", "User").HasName("fridge_shares_pkey");
                        j.ToTable("fridge_shares");
                        j.IndexerProperty<int>("FridgeId").HasColumnName("fridge_id");
                        j.IndexerProperty<string>("User")
                            .HasColumnType("character varying")
                            .HasColumnName("user");
                    });
        });

        modelBuilder.Entity<FridgeProduct>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.FridgeId }).HasName("fridge_products_pkey");

            entity.ToTable("fridge_products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.FridgeId).HasColumnName("fridge_id");
            entity.Property(e => e.Ammount).HasColumnName("ammount");

            entity.HasOne(d => d.Fridge).WithMany(p => p.FridgeProducts)
                .HasForeignKey(d => d.FridgeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fridge_products_fridge_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.FridgeProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fridge_products_product_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcode)
                .HasColumnType("character varying")
                .HasColumnName("barcode");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.User)
                .HasColumnType("character varying")
                .HasColumnName("user");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Category)
                .HasConstraintName("products_category_fkey");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recipes_pkey");

            entity.ToTable("recipes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("author");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.MarkdownPath)
                .HasColumnType("character varying")
                .HasColumnName("markdown_path");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.Author)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipes_author_fkey");

            entity.HasMany(d => d.Categories).WithMany(p => p.Reciepes)
                .UsingEntity<Dictionary<string, object>>(
                    "ReciepesCategory",
                    r => r.HasOne<FoodCategory>().WithMany()
                        .HasForeignKey("Category")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("reciepes_categories_category_fkey"),
                    l => l.HasOne<Recipe>().WithMany()
                        .HasForeignKey("Reciepe")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("reciepes_categories_reciepe_fkey"),
                    j =>
                    {
                        j.HasKey("Reciepe", "Category").HasName("reciepes_categories_pkey");
                        j.ToTable("reciepes_categories");
                        j.IndexerProperty<int>("Reciepe").HasColumnName("reciepe");
                        j.IndexerProperty<int>("Category").HasColumnName("category");
                    });
        });

        modelBuilder.Entity<RecipeChild>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("recipe_children");

            entity.Property(e => e.Child).HasColumnName("child");
            entity.Property(e => e.Multiplier).HasColumnName("multiplier");
            entity.Property(e => e.Recipe).HasColumnName("recipe");
        });

        modelBuilder.Entity<RecipesComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recipes_comments_pkey");

            entity.ToTable("recipes_comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Likes).HasColumnName("likes");
            entity.Property(e => e.Message)
                .HasColumnType("character varying")
                .HasColumnName("message");
            entity.Property(e => e.Recipe).HasColumnName("recipe");
            entity.Property(e => e.User)
                .HasColumnType("character varying")
                .HasColumnName("user");

            entity.HasOne(d => d.RecipeNavigation).WithMany(p => p.RecipesComments)
                .HasForeignKey(d => d.Recipe)
                .HasConstraintName("recipes_comments_recipe_fkey");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.RecipesComments)
                .HasForeignKey(d => d.User)
                .HasConstraintName("recipes_comments_user_fkey");
        });

        modelBuilder.Entity<RecipiesImage>(entity =>
        {
            entity.HasKey(e => new { e.Name, e.Recipe }).HasName("recipies_images_pkey");

            entity.ToTable("recipies_images");

            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Recipe).HasColumnName("recipe");
            entity.Property(e => e.ImagePath)
                .HasColumnType("character varying")
                .HasColumnName("image_path");

            entity.HasOne(d => d.RecipeNavigation).WithMany(p => p.RecipiesImages)
                .HasForeignKey(d => d.Recipe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipies_images_recipe_fkey");
        });

        modelBuilder.Entity<RecipiesIngredient>(entity =>
        {
            entity.HasKey(e => new { e.Reciepie, e.Category }).HasName("recipies_ingredients_pkey");

            entity.ToTable("recipies_ingredients");

            entity.Property(e => e.Reciepie).HasColumnName("reciepie");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.RecipiesIngredients)
                .HasForeignKey(d => d.Category)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipies_ingredients_category_fkey");

            entity.HasOne(d => d.ReciepieNavigation).WithMany(p => p.RecipiesIngredients)
                .HasForeignKey(d => d.Reciepie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipies_ingredients_reciepie_fkey");
        });

        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shopping_list_pkey");

            entity.ToTable("shopping_list");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.User)
                .HasColumnType("character varying")
                .HasColumnName("user");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.ShoppingLists)
                .HasForeignKey(d => d.User)
                .HasConstraintName("shopping_list_user_fkey");
        });

        modelBuilder.Entity<ShoppingListProduct>(entity =>
        {
            entity.HasKey(e => new { e.ShoppingListId, e.Category }).HasName("shopping_list_products_pkey");

            entity.ToTable("shopping_list_products");

            entity.Property(e => e.ShoppingListId).HasColumnName("shopping_list_id");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.ShoppingListProducts)
                .HasForeignKey(d => d.Category)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shopping_list_products_category_fkey");

            entity.HasOne(d => d.ShoppingList).WithMany(p => p.ShoppingListProducts)
                .HasForeignKey(d => d.ShoppingListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shopping_list_products_shopping_list_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("password");

            entity.HasMany(d => d.Follows).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersFollow",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Follows")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_follows_follows_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_follows_user_fkey"),
                    j =>
                    {
                        j.HasKey("User", "Follows").HasName("users_follows_pkey");
                        j.ToTable("users_follows");
                        j.IndexerProperty<string>("User")
                            .HasColumnType("character varying")
                            .HasColumnName("user");
                        j.IndexerProperty<string>("Follows")
                            .HasColumnType("character varying")
                            .HasColumnName("follows");
                    });

            entity.HasMany(d => d.RecipesNavigation).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "RecipesLike",
                    r => r.HasOne<Recipe>().WithMany()
                        .HasForeignKey("Recipe")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("recipes_likes_recipe_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("recipes_likes_user_fkey"),
                    j =>
                    {
                        j.HasKey("User", "Recipe").HasName("recipes_likes_pkey");
                        j.ToTable("recipes_likes");
                        j.IndexerProperty<string>("User")
                            .HasColumnType("character varying")
                            .HasColumnName("user");
                        j.IndexerProperty<int>("Recipe").HasColumnName("recipe");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.Follows)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersFollow",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_follows_user_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Follows")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_follows_follows_fkey"),
                    j =>
                    {
                        j.HasKey("User", "Follows").HasName("users_follows_pkey");
                        j.ToTable("users_follows");
                        j.IndexerProperty<string>("User")
                            .HasColumnType("character varying")
                            .HasColumnName("user");
                        j.IndexerProperty<string>("Follows")
                            .HasColumnType("character varying")
                            .HasColumnName("follows");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


