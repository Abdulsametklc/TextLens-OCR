using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using DocumentFormat.OpenXml.Packaging;

namespace OCR.Services
{
    public static class OcrService
    {
        private static string? _extractedTessdataPath;
        private static readonly object _lock = new();

        private static string GetTessdataPath()
        {
            // Önce uygulama dizininde ara
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var tessdataPath = Path.Combine(appDir, "tessdata");
            
            if (Directory.Exists(tessdataPath) && Directory.GetFiles(tessdataPath, "*.traineddata").Length > 0)
                return tessdataPath;
            
            // Development ortamında proje dizininde ara
            var projectPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            if (Directory.Exists(projectPath) && Directory.GetFiles(projectPath, "*.traineddata").Length > 0)
                return projectPath;
            
            // Gömülü kaynaklardan çıkar
            return ExtractEmbeddedTessdata();
        }

        private static string ExtractEmbeddedTessdata()
        {
            lock (_lock)
            {
                if (_extractedTessdataPath != null && Directory.Exists(_extractedTessdataPath))
                    return _extractedTessdataPath;

                var tempDir = Path.Combine(Path.GetTempPath(), "TextLens_OCR");
                var tessdataDir = Path.Combine(tempDir, "tessdata");
                
                Directory.CreateDirectory(tessdataDir);

                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames = assembly.GetManifestResourceNames();

                foreach (var resourceName in resourceNames)
                {
                    if (resourceName.EndsWith(".traineddata"))
                    {
                        // Kaynak adından dosya adını çıkar (örn: OCR.tessdata.eng.traineddata -> eng.traineddata)
                        var parts = resourceName.Split('.');
                        var fileName = parts.Length >= 2 
                            ? $"{parts[^2]}.traineddata" 
                            : resourceName;
                        
                        var filePath = Path.Combine(tessdataDir, fileName);
                        
                        // Dosya zaten varsa ve boyutu doğruysa atla
                        if (File.Exists(filePath))
                            continue;

                        using var stream = assembly.GetManifestResourceStream(resourceName);
                        if (stream != null)
                        {
                            using var fileStream = File.Create(filePath);
                            stream.CopyTo(fileStream);
                        }
                    }
                }

                _extractedTessdataPath = tessdataDir;
                return tessdataDir;
            }
        }

        public static async Task<string> ProcessFileAsync(string filePath, string lang = "eng+tur")
        {
            return await Task.Run(() => ProcessFile(filePath, lang));
        }

        public static string ProcessFile(string filePath, string lang = "eng+tur")
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".txt" => ReadTextFile(filePath),
                ".docx" => ReadDocxFile(filePath),
                ".png" or ".jpg" or ".jpeg" or ".bmp" or ".tiff" or ".webp" => OcrImage(filePath, lang),
                ".pdf" => "[PDF desteği yakında eklenecek]",
                _ => throw new NotSupportedException($"Desteklenmeyen dosya formatı: {extension}")
            };
        }

        private static string ReadTextFile(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        private static string ReadDocxFile(string filePath)
        {
            var sb = new StringBuilder();
            
            using (var wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document?.Body;
                if (body != null)
                {
                    sb.Append(body.InnerText);
                }
            }
            
            return sb.ToString();
        }

        private static string OcrImage(string filePath, string lang)
        {
            var tessdataPath = GetTessdataPath();
            
            using var engine = new TesseractEngine(tessdataPath, lang, EngineMode.Default);
            using var img = Pix.LoadFromFile(filePath);
            using var page = engine.Process(img);
            
            return page.GetText().Trim();
        }
    }
}

