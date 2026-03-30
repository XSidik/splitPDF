# SplitPDFApp

A simple, modern PDF splitting web application built with .NET 10 and Tailwind CSS.

## Features
- **Upload PDF**: Easily upload multi-page PDF files.
- **Split PDF**: Automatically split each page into a separate PDF file.
- **Download Results**: Download all split pages as a single ZIP archive.
- **Modern UI**: Clean and simple interface styled with Tailwind CSS.

## Technology Stack
- **Backend**: .NET 10 (ASP.NET Core MVC)
- **PDF Processing**: [PdfSharpCore](https://github.com/ststeiger/PdfSharpCore)
- **Archiving**: [DotNetZip](https://github.com/haf/DotNetZip.Semverd)
- **Styling**: [Tailwind CSS](https://tailwindcss.com/)

## Getting Started

### Prerequisites
- .NET 10 SDK

### Installation
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd SplitPDFApp
   ```
2. Run the application:
   ```bash
   dotnet run
   ```
3. Open your browser and navigate to `http://localhost:5000`.

## Project Structure
- `Controllers/HomeController.cs`: Contains the core logic for file handling and PDF splitting.
- `Views/Home/Index.cshtml`: The main user interface for uploading files.
- `wwwroot/uploads/`: Temporary storage for processed files (automatically created).

## License
MIT
