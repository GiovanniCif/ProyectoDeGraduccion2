namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file, string? subFolder = null);
        Task<bool> DeleteFileAsync(string filePath);
        Task<FileUploadResult[]> UploadMultipleFilesAsync(IFormFile[] files, string? subFolder = null);
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FilePath { get; set; }
        public string? Url { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
