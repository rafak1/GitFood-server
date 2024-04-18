using Server.Data.Models;
using Server.ViewModels.Categories;

namespace Server.Logic.Abstract.Managers;

public interface ICategoryManager
{
    public Task AddCategoryAsync(CategoryViewModel category);
    public Task<Category[]> GetCategoriesAsync(string name);
    public Task DeleteCategoryAsync(int id);
}