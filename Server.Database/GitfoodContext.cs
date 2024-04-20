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


    public virtual DbSet<Barcode> Barcodes { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Fridge> Fridges { get; set; }

    public virtual DbSet<FridgeUnit> FridgeUnits { get; set; }

    public virtual DbSet<Product> Products { get; set; }

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
        modelBuilder.Entity<Barcode>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("Barcode_pkey");

            entity.ToTable("barcodes");

            entity.Property(e => e.Key)
                .HasColumnType("character varying")
                .HasColumnName("key");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.User)
                .HasColumnType("character varying")
                .HasColumnName("user");

            entity.HasOne(d => d.Product).WithMany(p => p.Barcodes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Barcode_ProductId_fkey");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Barcodes)
                .HasForeignKey(d => d.User)
                .HasConstraintName("barcodes_user_fkey");
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
        });

        modelBuilder.Entity<Fridge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fridge_pkey");

            entity.ToTable("fridge");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserLogin)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("user_login");

            entity.HasOne(d => d.Product).WithMany(p => p.Fridges)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fridge_product_id_fkey");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.Fridges)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fridge_user_login_fkey");
        });

        modelBuilder.Entity<FridgeUnit>(entity =>
        {
            entity.HasKey(e => new { e.FridgeProductId, e.Unit }).HasName("fridge_units_pkey");

            entity.ToTable("fridge_units");

            entity.Property(e => e.FridgeProductId).HasColumnName("fridge_product_id");
            entity.Property(e => e.Unit)
                .HasColumnType("character varying")
                .HasColumnName("unit");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.FridgeProduct).WithMany(p => p.FridgeUnits)
                .HasForeignKey(d => d.FridgeProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fridge_units_fridge_product_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasMany(d => d.Categories).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductsCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("products_categories_category_id_fkey"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("products_categories_product_id_fkey"),
                    j =>
                    {
                        j.HasKey("ProductId", "CategoryId").HasName("products_categories_pkey");
                        j.ToTable("products_categories");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}