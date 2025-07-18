    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using NAudio.Wave;
    using NAudio.Lame;
    
    namespace Venkat.WavToMP3Converter
    {
        /// <summary>
        /// Provides functionality to convert WAV audio files to MP3 format using NAudio and LAME.
        /// </summary>
        public class WavToMp3Converter
        {
            private readonly ILogger<WavToMp3Converter> _logger;
    
            /// <summary>
            /// Initializes a new instance of the <see cref="WavToMp3Converter"/> class.
            /// </summary>
            /// <param name="logger">Logger instance for logging information and errors.</param>
            public WavToMp3Converter(ILogger<WavToMp3Converter> logger)
            {
                _logger = logger;
            }
    
            /// <summary>
            /// Azure Function that converts an uploaded WAV file to MP3 format.
            /// </summary>
            /// <param name="req">HTTP request containing the WAV file in multipart/form-data.</param>
            /// <param name="log">Logger for function-level logging.</param>
            /// <returns>
            /// Returns an <see cref="IActionResult"/> containing the converted MP3 file,
            /// or an error message if the conversion fails or the input is invalid.
            /// </returns>
            /// <remarks>
            /// The function expects a POST request with a WAV file uploaded under the form field "file".
            /// </remarks>
            [FunctionName("ConvertWavToMp3")]
            public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
                HttpRequest req)
            {
                _logger.LogInformation("WAV to MP3 conversion function processed a request.");
    
                // Validate content type
                if (!req.ContentType.StartsWith("multipart/form-data"))
                {
                    return new BadRequestObjectResult("Please upload a WAV file.");
                }
    
                // Read form data and retrieve the uploaded file
                var formData = await req.ReadFormAsync();
                var file = formData.Files.GetFile("file");
    
                // Validate file existence and extension
                if (file == null || file.Length == 0 || Path.GetExtension(file.FileName)?.ToLower() != ".wav")
                {
                    return new BadRequestObjectResult("Please upload a valid WAV file.");
                }
    
                try
                {
                    // Read the WAV file stream
                    using var wavStream = file.OpenReadStream();
                    using var wavReader = new WaveFileReader(wavStream);
    
                    // Convert WAV to MP3 using LAME encoder
                    using var outputStream = new MemoryStream();
                    using (var mp3Writer = new LameMP3FileWriter(outputStream, wavReader.WaveFormat, LAMEPreset.VBR_90))
                    {
                        wavReader.CopyTo(mp3Writer);
                    }
    
                    // Prepare the MP3 file for download
                    outputStream.Position = 0;
                    return new FileStreamResult(outputStream, "audio/mpeg")
                    {
                        FileDownloadName = Path.ChangeExtension(file.FileName, ".mp3")
                    };
                }
                catch (Exception ex)
                {
                    // Log and return error if conversion fails
                    _logger.LogError(ex, "Error during WAV to MP3 conversion.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
