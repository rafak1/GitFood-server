using Server.Logic.Abstract.Managers;
using Server.Database;
using Server.ViewModels.Categories;
using Server.Logic.Abstract;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Users;
using Server.ViewModels;

namespace Server.Logic.Managers;

internal class CategoryManager : ICategoryManager
{
    private readonly GitfoodContext _dbInfo;

    public CategoryManager(GitfoodContext dbInfo) 
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult<int>> AddNewCategoryRequestAsync(CategoryViewModel category, string user)
    {
        if (!Enum.IsDefined(typeof(Units), category.Unit))
            return new ManagerActionResult<int>(0, ResultEnum.BadRequest);


        var isElevated = Enum.IsDefined(typeof(ElevatedUsers), user);

        var status = isElevated ? CategoryStatus.Confirmed.ToString() : CategoryStatus.UnConfirmed.ToString();

        var transaction = _dbInfo.Database.BeginTransaction();

        await _dbInfo.Categories.AddAsync(new Category() {
            Name = category.Name,
            Unit = category.Unit,
            Status = status
        });

        await _dbInfo.SaveChangesAsync();
        var id = await _dbInfo.Categories.Where(x => x.Name == category.Name).Select(x => x.Id).FirstOrDefaultAsync();

        if(!isElevated){
            await _dbInfo.AddCategoriesRequests.AddAsync(new AddCategoriesRequest() {
                User = user,
                Request = id,
                Datetime = DateOnly.FromDateTime(DateTime.Now)
            });
            await _dbInfo.SaveChangesAsync();
        }

        await transaction.CommitAsync();

        return new ManagerActionResult<int>(id, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Category[]>> GetCategoriesAsync() 
    {
        var categories = await _dbInfo.Categories.ToArrayAsync();
        return new ManagerActionResult<Category[]>(categories, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Category[]>> GetVerifiedCategoriesAsync() 
    {
        var categories = await _dbInfo.Categories.Where(x => x.Status == CategoryStatus.Confirmed.ToString()).ToArrayAsync();
        return new ManagerActionResult<Category[]>(categories, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteCategoryAsync(int id) 
    {
        await _dbInfo.Categories.Where(x => x.Id == id).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<string[]>> GetUnitsAsync() 
    {
        var units = Enum.GetNames(typeof(Units));
        return new ManagerActionResult<string[]>(units, ResultEnum.OK);
    }


    public async Task<IManagerActionResult<IdExtendedViewModel<CategoryViewModel>[]>> GetSuggestionsAsync(string name, int resultsCount)
    {
        IQueryable<Category> query = _dbInfo.Categories.Include(x => x.Products);

    
        if(!string.IsNullOrEmpty(name))
            query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));

        var result = await query
            .OrderByDescending(x => x.Products.Count)
            .Take(resultsCount)
            .Select(x => new IdExtendedViewModel<CategoryViewModel>() {
                Id = x.Id,
                InnerInformation = new CategoryViewModel() {
                    Name = x.Name,
                    Unit = x.Unit,
                    Status = x.Status
                }
            }).ToArrayAsync();
        
        return new ManagerActionResult<IdExtendedViewModel<CategoryViewModel>[]>(result, ResultEnum.OK);
    }
}