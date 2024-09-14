using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DocumentStorage;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController(IKernelMemory kernelMemory) : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create()
        {
            await kernelMemory.ImportWebPageAsync("https://en.wikipedia.org/wiki/.NET");
            return Created();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Query(string query)
        {
            var answer = await kernelMemory.AskAsync(query);
            return Ok(answer);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await kernelMemory.ImportDocumentAsync(file.OpenReadStream(),fileName:file.FileName);
            return Created();
        }
    }
}
