using Server.Data.Models;
using Server.ViewModels.Barcodes;

namespace Server.Logic.Abstract;

public interface IBarcodeManager
{
    public Task AddBarcodeAsync(BarcodeViewModel barcode);
    public Task<Barcode> GetBarcodeAsync(string barcodeKey);
    public Task DeleteBarcodeAsync(string barcodeKey);
}