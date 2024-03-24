using Microsoft.AspNetCore.Mvc;
using Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Server.DataModel;

namespace Server.Controllers;

[ApiController]
public class BarcodeController : Controller
{
    private const string _controllerRoute = "/barcode";

    private readonly GitfoodContext _dbInfo;

    public BarcodeController(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    
    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            Key = barcode.BarcodeNumber,
            ProductId = barcode.ProductId
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetBarcode(string barcodeNumber) 
    {
        return Ok(await _dbInfo.Barcodes.FirstOrDefaultAsync(x => x.Key == barcodeNumber));
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteBarcode(string barcodeName) 
    {
        await _dbInfo.Barcodes.Where(x => x.Key == barcodeName).ExecuteDeleteAsync();
        return Ok();
    }
}
