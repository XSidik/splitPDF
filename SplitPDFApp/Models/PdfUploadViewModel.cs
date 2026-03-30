using System.ComponentModel.DataAnnotations;

namespace SplitPDFApp.Models;

public class PdfUploadViewModel
{
    [Required(ErrorMessage = "Please select a PDF file.")]
    public IFormFile PdfFile { get; set; } = default!;

    [Display(Name = "Split Mode")]
    public SplitMode Mode { get; set; } = SplitMode.SinglePages;
}

public enum SplitMode
{
    [Display(Name = "Individual Pages")]
    SinglePages
}
