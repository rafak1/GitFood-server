using Server.Logic.Abstract.Managers;
using Server.Database;
using Server.ViewModels.Categories;
using Server.Logic.Abstract;
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

    public async Task<IManagerActionResult> AddCategoryAsync(CategoryViewModel category) 
    {
        await _dbInfo.Categories.AddAsync(new Category() {
            Name = category.Name,
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Category[]>> GetCategoriesAsync(string name) 
    {
        var categories = await _dbInfo.Categories.Where(x => x.Name.Contains(name)).ToArrayAsync();
        return new ManagerActionResult<Category[]>(categories, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteCategoryAsync(int id) 
    {
        await _dbInfo.Categories.Where(x => x.Id == id).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }
}