using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.ViewModels;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
public class HomeController : Controller
{
    private const string _controllerRoute = "/barcode";

    private readonly DataContext _dbInfo;

    public HomeController(DataContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    
    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            BarcodeBytes = barcode.BarcodeBytes,
            BarcodeNumber = barcode.BarcodeNumber,
            Name = barcode.Name
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetBarcode(string barcodeNumber) 
    {
        return Ok(await _dbInfo.Barcodes.FirstOrDefaultAsync(x => x.BarcodeNumber == barcodeNumber));
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteBarcode(string barcodeName) 
    {
        await _dbInfo.Barcodes.Where(x => x.BarcodeNumber == barcodeName).ExecuteDeleteAsync();
        return Ok();
    }
}
