using Server.Data.Models;
using Server.Logic.Abstract.Managers;
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

    public async Task AddBarcodeAsync(BarcodeViewModel barcode)
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            Key = barcode.BarcodeNumber,
            ProductId = barcode.ProductId,
        });
        await _dbInfo.SaveChangesAsync();
    }

    public async Task DeleteBarcodeAsync(string barcodeKey)
        => await _dbInfo.Barcodes.Where(x => x.Key == barcodeKey).ExecuteDeleteAsync();

    public async Task<Barcode> GetBarcodeAsync(string barcodeKey)
        => await _dbInfo.Barcodes.FirstOrDefaultAsync(x => x.Key == barcodeKey);

    public async Task<IDictionary<string, Barcode>> SuggestBarcodeAsync(string barcodeName) 
    {
        return (await _dbInfo.Barcodes
            .Where(x => x.Key == barcodeName)
            .GroupBy(x => x.Key)
            .OrderByDescending(x => x.Count())
            .FirstOrDefaultAsync()).ToDictionary(x => x.Key);
    }
}