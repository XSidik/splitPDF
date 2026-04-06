using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SplitPDFApp.Models;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Ionic.Zip;

namespace SplitPDFApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _env;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PdfUploadViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> SplitPDF(PdfUploadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (model.PdfFile == null || model.PdfFile.Length == 0 || !model.PdfFile.FileName.ToLower().EndsWith(".pdf"))
        {
            return BadRequest("Please upload a valid PDF file.");
        }

        var tempFolder = Path.Combine(_env.WebRootPath, "uploads", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempFolder);

        try
        {
            var fileName = Path.GetFileNameWithoutExtension(model.PdfFile.FileName);
            var filePath = Path.Combine(tempFolder, model.PdfFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.PdfFile.CopyToAsync(stream);
            }

            // Split PDF
            var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            var pageCount = inputDocument.PageCount;
            var splitFiles = new List<object>();
            var allFilePaths = new List<string>();

            for (int i = 0; i < pageCount; i++)
            {
                var outputDocument = new PdfDocument();
                outputDocument.AddPage(inputDocument.Pages[i]);
                
                var splitFileName = $"{fileName}_page_{i + 1}.pdf";
                var splitPath = Path.Combine(tempFolder, splitFileName);
                outputDocument.Save(splitPath);
                allFilePaths.Add(splitPath);

                var bytes = await System.IO.File.ReadAllBytesAsync(splitPath);
                var base64 = Convert.ToBase64String(bytes);

                splitFiles.Add(new {
                    pageNumber = i + 1,
                    fileName = splitFileName,
                    data = base64
                });
            }

            // Zip files
            var zipFileName = $"{fileName}_split.zip";
            var zipPath = Path.Combine(tempFolder, zipFileName);

            using (var zip = new ZipFile())
            {
                foreach (var file in allFilePaths)
                {
                    zip.AddFile(file, "");
                }
                zip.Save(zipPath);
            }

            var zipBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
            var zipBase64 = Convert.ToBase64String(zipBytes);

            return Json(new {
                success = true,
                fileName = model.PdfFile.FileName,
                pageCount = pageCount,
                pages = splitFiles,
                zipData = zipBase64,
                zipFileName = zipFileName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error splitting PDF");
            return StatusCode(500, "An error occurred while processing the PDF.");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
