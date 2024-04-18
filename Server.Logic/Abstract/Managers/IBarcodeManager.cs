using Server.Data.Models;
using Server.ViewModels.Barcodes;

namespace Server.Logic.Abstract.Managers;

public interface IBarcodeManager
{
    public Task AddBarcodeAsync(BarcodeViewModel barcode);
    public Task<Barcode> GetBarcodeAsync(string barcodeKey);
    public Task DeleteBarcodeAsync(string barcodeKey);
    public Task<IDictionary<string, Barcode>> SuggestBarcodeAsync(string barcodeKey);
}