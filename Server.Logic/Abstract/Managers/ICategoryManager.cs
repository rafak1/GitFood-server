using Server.Data.Models;
using Server.ViewModels.Categories;

namespace Server.Logic.Abstract.Managers;

public interface ICategoryManager
{
    public Task<IManagerActionResult> AddCategoryAsync(CategoryViewModel category);
    public Task<IManagerActionResult<Category[]>> GetCategoriesAsync(string name);
    public Task<IManagerActionResult> DeleteCategoryAsync(int id);
}