using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridwan.Service.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IProcessingUtil _offenceService;

        public ValuesController( IProcessingUtil offenceService)
        {
            _offenceService = offenceService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadOffenceFile()
        {
            try
            {
                var request = FileUploadRequest.UploadFile(Request);

                if (request == null)
                    throw new AppException("No valid file Selected");

               await _offenceService.ProcessDocument(request, 4, 3);

                return Ok(new { responseCode = ResponseCodes.Success, responseDescription = "Offence Uploaded Successfully" });
            }
            catch (AppException ex)
            {
                Console.WriteLine($"Unable to upload offence: {ex.Message} {ex.StackTrace}");
                return BadRequest(new { ResponseCodes.InvalidRequest, ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occured: {ex.Message} {ex.StackTrace}");
                return BadRequest(new { ResponseCodes.InvalidRequest, message = "An unexpected error occured. Please try again!" });
            }
        }

    }
}
