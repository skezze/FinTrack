using Microsoft.AspNetCore.Mvc;
using FinTrack.API.Services.Interfaces;
using FinTrack.API.Models;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BouncyCastleController : ControllerBase
    {
        private readonly IBouncyCastleService _bouncyCastleService;

        public BouncyCastleController(IBouncyCastleService bouncyCastleService)
        {
            _bouncyCastleService = bouncyCastleService;
        }

        [HttpPost]
        public IActionResult GenerateKeys()
        {
            _bouncyCastleService.GenerateRsaKeys();
            return Ok("RSA keys generated successfully.");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult SignDocument([FromForm] SignDocumentRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var signaturePath = _bouncyCastleService.SignDocument(filePath);
            return Ok(new { message = "Document signed successfully.", signaturePath });
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult VerifySignature([FromForm] VerifySignatureRequest request)
        {
            var file = request.File;
            var signature = request.Signature;

            if (file == null || signature == null || file.Length == 0 || signature.Length == 0)
                return BadRequest("Invalid file or signature.");

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            var signaturePath = Path.Combine(Path.GetTempPath(), signature.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            using (var stream = new FileStream(signaturePath, FileMode.Create))
            {
                signature.CopyTo(stream);
            }

            var isValid = _bouncyCastleService.VerifySignature(filePath, signaturePath);
            return Ok(new { isValid });
        }
    }
}
