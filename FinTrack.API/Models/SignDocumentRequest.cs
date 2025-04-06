using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Models
{
    public class SignDocumentRequest
    {
        [FromForm]
        public IFormFile File { get; set; }
    }
}
