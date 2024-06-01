using Server.Logic.Abstract.Managers;
using Server.Database;
using Server.ViewModels.Categories;
using Server.Logic.Abstract;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Users;
using Server.ViewModels;
using Server.ViewModels.FoodCategories;

namespace Server.Logic.Managers;

internal class FoodCategoryManager : IFoodCategoryManager
{
    private readonly GitfoodContext _dbInfo;

    public FoodCategoryManager(GitfoodContext dbInfo)
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult<int>> AddFoodCategoryAsync(string name, string description, string user)
        => await new DatabaseExceptionHandler<int>().HandleExceptionsAsync(async () => await AddFoodCategoryInternalAsync(name, description, user));

    private async Task<IManagerActionResult<int>> AddFoodCategoryInternalAsync(string name, string description, string user)
    {
        if (Enum.IsDefined(typeof(ElevatedUsers), user))
        {
            var category = new FoodCategory
            {
                Name = name,
                Description = description
            };

            await _dbInfo.FoodCategories.AddAsync(category);
            await _dbInfo.SaveChangesAsync();

            return new ManagerActionResult<int>(category.Id, ResultEnum.OK);
        }
        else
        {
            return new ManagerActionResult<int>(-1, ResultEnum.Unauthorizated);
        }
    }

    public async Task<IManagerActionResult<FoodCategory[]>> GetAllFoodCategoriesAsync()
    {
        var categories = await _dbInfo.FoodCategories.ToArrayAsync();
        return new ManagerActionResult<FoodCategory[]>(categories, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> RemoveFoodCategoryAsync(int foodCategoryId, string user)
    {
        if (Enum.IsDefined(typeof(ElevatedUsers), user))
        {
            var category = await _dbInfo.FoodCategories.FindAsync(foodCategoryId);
            if (category == null)
            {
                return new ManagerActionResult(ResultEnum.NotFound);
            }

            _dbInfo.FoodCategories.Remove(category);
            await _dbInfo.SaveChangesAsync();

            return new ManagerActionResult(ResultEnum.OK);
        }
        else
        {
            return new ManagerActionResult(ResultEnum.Unauthorizated);
        }
    }

    public async Task<IManagerActionResult<IdExtendedViewModel<FoodCategoryViewModel>[]>> GetFoodCategorySuggestionsAsync(string name, int resultsCount)
    {
        IQueryable<FoodCategory> query = _dbInfo.FoodCategories.Include(x => x.Reciepes);

    
        if(!string.IsNullOrEmpty(name))
            query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));

        var result = await query
            .OrderByDescending(x => x.Reciepes.Count)
            .Take(resultsCount)
            .Select(x => new IdExtendedViewModel<FoodCategoryViewModel> {
                Id = x.Id,
                InnerInformation = new FoodCategoryViewModel {
                    Name = x.Name,
                    Description = x.Description
                }
            }).ToArrayAsync();
        return new ManagerActionResult<IdExtendedViewModel<FoodCategoryViewModel>[]>(result, ResultEnum.OK);
    }

}
