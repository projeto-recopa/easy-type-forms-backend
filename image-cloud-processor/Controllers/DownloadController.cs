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
    public class DownloadController : ControllerBase
    {
        //private readonly AppDbContext _context;

        private readonly long _fileSizeLimit;
        private readonly ILogger<UploadController> _logger;
        private readonly DownloadService _downloadService;

        private readonly string[] _permittedExtensions = { ".png", ".jpeg", ".jpg" };
        private readonly string _targetFilePath;

        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public DownloadController(ILogger<UploadController> logger,
            DownloadService downloadService,
            IConfiguration config)
        {
            _logger = logger;
            _downloadService = downloadService;

            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();
        }

        // GET api/<DocumentController>/5
        [HttpGet("options/{id}/field/{field}")]
        public ActionResult Get(string id,int field)
        {
            byte[] bytesInStream = this._downloadService.DownloadOptionsImage(id, field);

            var memory = new MemoryStream(bytesInStream);
            return File(memory,  "image/png", $"{id}.png");

        }

    }

}
