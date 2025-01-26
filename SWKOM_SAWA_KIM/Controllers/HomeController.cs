using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWKOM_SAWA_KIM.DAL.Data;
using SWKOM_SAWA_KIM.BLL.Services;
using SWKOM_SAWA_KIM.BLL.DTOs;
using SWKOM_SAWA_KIM.BLL.Validators;
using SWKOM_SAWA_KIM.Minio;
using SWKOM_SAWA_KIM.RabbitMQ;

namespace SWKOM_SAWA_KIM.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IMinioService _minioService;
        private readonly IQueueProducer _queueProducer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IDocumentService documentService, IMinioService minioService, IQueueProducer queueProducer, ILogger<HomeController> logger)
        {
            _documentService = documentService;
            _minioService = minioService;
            _queueProducer = queueProducer;
            _logger = logger;

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }

        [HttpGet("documents")]
        public async Task<IActionResult> GetDocuments()
        {
            try
            {
                _logger.LogInformation("Getting all documents");
                // Fetch documents from the database
                var documents = await _documentService.GetAllDocumentsAsync();

                // Return only filenames
                var response = documents.Select(d => new { d.Filename });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all documents");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("search")]
        public IActionResult Search([FromQuery] string query)
        {
            if(string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }

            try
            {
                _logger.LogInformation("Searching documents");
                var results = _documentService.SearchDocuments(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching documents");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Post([FromForm] IFormFile file)
        {
            // not using custom validator yet
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                return BadRequest("Only PDF files are allowed.");

            // guid id gets created as documentdto default val

            _logger.LogInformation("Uploading document");
            var documentDTO = new DocumentDTO
            {
                Filename = file.FileName,
                // Content = "Hard-coded Content" commented out, cuz nullable anyway, will be set after OCR
            };

            Console.WriteLine("DOCUMENT NAME: " + documentDTO.Filename);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using var fileStream = file.OpenReadStream();

                await _minioService.UploadDocumentAsync(documentDTO.Id, fileStream);

                await _documentService.AddDocumentAsync(documentDTO);

                _queueProducer.SendToTaskQueue(documentDTO.Filename, documentDTO.Id);

                return Ok(new { message = "File uploaded successfully", id = documentDTO.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the document");
                return BadRequest(ex.Message);
            }
        }
    }
}
