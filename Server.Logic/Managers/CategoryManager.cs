using Server.Logic.Abstract.Managers;
using Server.Database;
using Server.ViewModels.Categories;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Logic.Managers;

internal class CategoryManager : ICategoryManager
{
    private readonly GitfoodContext _dbInfo;

    public CategoryManager(GitfoodContext dbInfo) 
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task AddCategoryAsync(CategoryViewModel category) 
    {
        await _dbInfo.Categories.AddAsync(new Category() {
            Name = category.Name,
        });
        await _dbInfo.SaveChangesAsync();
    }

    public async Task<Category[]> GetCategoriesAsync(string name) 
        => await _dbInfo.Categories.Where(x => x.Name.Contains(name)).ToArrayAsync();

    public async Task DeleteCategoryAsync(int id) 
        => await _dbInfo.Categories.Where(x => x.Id == id).ExecuteDeleteAsync();

}