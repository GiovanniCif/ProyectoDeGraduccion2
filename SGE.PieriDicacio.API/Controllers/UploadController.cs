using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IFileUploadService _uploadService;

        public UploadController(IFileUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("voucher")]
        [ProducesResponseType(typeof(FileUploadResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> UploadVoucher(IFormFile file)
        {
            var result = await _uploadService.UploadFileAsync(file, "vouchers");
            
            if (!result.Success)
                return BadRequest(new { mensaje = result.ErrorMessage });

            return Ok(result);
        }

        [HttpPost("imagen")]
        [ProducesResponseType(typeof(FileUploadResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await _uploadService.UploadFileAsync(file, "imagenes");
            
            if (!result.Success)
                return BadRequest(new { mensaje = result.ErrorMessage });

            return Ok(result);
        }

        [HttpPost("multiple")]
        [ProducesResponseType(typeof(FileUploadResult[]), 200)]
        public async Task<IActionResult> UploadMultiple(IFormFile[] files)
        {
            var results = await _uploadService.UploadMultipleFilesAsync(files, "otros");
            return Ok(results);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> DeleteFile([FromQuery] string path)
        {
            var result = await _uploadService.DeleteFileAsync(path);
            return Ok(new { success = result, mensaje = result ? "Archivo eliminado" : "No se pudo eliminar el archivo" });
        }
    }
}
