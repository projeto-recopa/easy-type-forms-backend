using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using image_cloud_processor.Filters;
using image_cloud_processor.Repository;
using image_cloud_processor.Service;
using image_cloud_processor.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace image_cloud_processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        //private readonly AppDbContext _context;

        private readonly long _fileSizeLimit;
        private readonly ILogger<UploadController> _logger;
        private readonly UploadService _uploadService;

        private readonly string[] _permittedExtensions = { ".png", ".jpeg", ".jpg" };
        private readonly string _targetFilePath;

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public UploadController(ILogger<UploadController> logger,
            UploadService uploadService,
            IConfiguration config)
        {
            _logger = logger;
            _uploadService = uploadService;

            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();
        }

        #region snippet_UploadGridFS
        [HttpPost()]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadGridFS()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();
            var result = MongoDB.Bson.ObjectId.Empty.ToString();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File",
                            $"The request couldn't be processed (Error 2).");
                        // Log error

                        return BadRequest(ModelState);
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        //result = this._documentosRepository.AttachFile(streamedFileContent);
                        result = this._uploadService.CreateDocumentFromFile(streamedFileContent);

                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return Created(nameof(UploadController), new { Id = result });
        }
        #endregion

        // GET api/<DocumentController>/5
        [HttpGet("download/{id}")]
        public ActionResult Get(string id, [FromQuery] int edit = -1)
        {
            byte[] bytesInStream = this._uploadService.DownloadImage(id, edit);

            var memory = new MemoryStream(bytesInStream);
            return File(memory,  "image/png", $"{id}.png");

        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }

}
