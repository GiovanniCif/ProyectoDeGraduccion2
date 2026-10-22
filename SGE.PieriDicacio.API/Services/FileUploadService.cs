using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
        private readonly string[] _allowedImageTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        private readonly long _maxFileSize = 10 * 1024 * 1024;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrEmpty(_environment.WebRootPath))
                return _environment.WebRootPath;

            var contentRoot = _environment.ContentRootPath ?? AppContext.BaseDirectory;
            var wwwRootPath = Path.Combine(contentRoot, "wwwroot");

            if (!Directory.Exists(wwwRootPath))
            {
                Directory.CreateDirectory(wwwRootPath);
            }

            return wwwRootPath;
        }

        public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string? subFolder = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new FileUploadResult { Success = false, ErrorMessage = "El archivo está vacío" };

                if (file.Length > _maxFileSize)
                    return new FileUploadResult { Success = false, ErrorMessage = $"El archivo es muy grande. Máximo {_maxFileSize / 1024 / 1024}MB" };

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                    return new FileUploadResult { Success = false, ErrorMessage = $"Tipo de archivo no permitido. Permitidos: {string.Join(", ", _allowedExtensions)}" };

                var webRootPath = GetWebRootPath();
                var uploadsFolder = Path.Combine(webRootPath, "uploads");
                if (!string.IsNullOrEmpty(subFolder))
                {
                    uploadsFolder = Path.Combine(uploadsFolder, subFolder);
                }

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var relativePath = string.IsNullOrEmpty(subFolder)
                    ? $"/uploads/{uniqueFileName}"
                    : $"/uploads/{subFolder}/{uniqueFileName}";

                return new FileUploadResult
                {
                    Success = true,
                    OriginalFileName = file.FileName,
                    FileName = uniqueFileName,
                    FilePath = filePath,
                    Url = relativePath,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Error al subir el archivo: {ex.Message}"
                };
            }
        }

        public async Task<FileUploadResult[]> UploadMultipleFilesAsync(IFormFile[] files, string? subFolder = null)
        {
            var results = new List<FileUploadResult>();
            
            foreach (var file in files)
            {
                var result = await UploadFileAsync(file, subFolder);
                results.Add(result);
            }

            return results.ToArray();
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return Task.FromResult(false);

                var webRootPath = GetWebRootPath();
                var fullPath = Path.Combine(webRootPath, filePath.TrimStart('/', '\\'));
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
