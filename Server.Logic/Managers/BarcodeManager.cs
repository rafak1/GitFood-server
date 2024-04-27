using Server.Data.Models;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract;
using Server.Database;
using Server.ViewModels.Barcodes;
using Microsoft.EntityFrameworkCore;

namespace Server.Logic.Managers;

internal class BarcodeManager : IBarcodeManager
{
    private readonly GitfoodContext _dbInfo;

    public BarcodeManager(GitfoodContext dbInfo) 
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult> AddBarcodeAsync(BarcodeViewModel barcode, string user)
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            Key = barcode.BarcodeNumber,
            ProductId = barcode.ProductId,
            User = user
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);

    }

    public async Task<IManagerActionResult> DeleteBarcodeAsync(string barcodeKey) 
    {
        await _dbInfo.Barcodes.Where(x => x.Key == barcodeKey).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Barcode>> GetBarcodeAsync(string barcodeKey) 
    {
        var barcode = await _dbInfo.Barcodes.FirstOrDefaultAsync(x => x.Key == barcodeKey);
        return new ManagerActionResult<Barcode>(barcode, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<IDictionary<string, Barcode>>> SuggestBarcodeAsync(string barcodeName) 
    {
        var barcodes = (await _dbInfo.Barcodes
            .Where(x => x.Key == barcodeName)
            .GroupBy(x => x.Key)
            .OrderByDescending(x => x.Count())
            .FirstOrDefaultAsync()).ToDictionary(x => x.Key);
        return new ManagerActionResult<IDictionary<string, Barcode>>(barcodes, ResultEnum.OK);
    }
}