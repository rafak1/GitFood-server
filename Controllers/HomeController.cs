using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;
using Server.Data.Models;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Server.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _dbInfo;

    public HomeController(ILogger<HomeController> logger, DataContext database)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    [Route("/addRandomBarcode")]
    public async Task<IActionResult> AddRandomBarcode() 
    {
        await _dbInfo.Barcodes.AddAsync(new Data.Models.Barcode{ BarcodeBytes = new byte[10], BarcodeNumber = "101010", Name = "dziwna rzecz"});
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    
    [HttpPost]
    [Route("/addBarcode")]
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

    [Route("/getBarcodes")]
    public IActionResult GetBarcodes() 
    {
        return Ok(_dbInfo.Barcodes.ToArray());
    }
}
