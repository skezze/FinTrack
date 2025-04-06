using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Models
{
    public class VerifySignatureRequest
    {
        [FromForm]
        public IFormFile File { get; set; }

        [FromForm]
        public IFormFile Signature { get; set; }
    }
}
