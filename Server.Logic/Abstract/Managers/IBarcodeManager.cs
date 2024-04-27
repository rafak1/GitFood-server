using Server.Data.Models;
using Server.ViewModels.Barcodes;

namespace Server.Logic.Abstract.Managers;

public interface IBarcodeManager
{
    public Task<IManagerActionResult> AddBarcodeAsync(BarcodeViewModel barcode, string user);
    public Task<IManagerActionResult<Barcode>> GetBarcodeAsync(string barcodeKey);
    public Task<IManagerActionResult> DeleteBarcodeAsync(string barcodeKey);
    public Task<IManagerActionResult<IDictionary<string, Barcode>>> SuggestBarcodeAsync(string barcodeKey);
}