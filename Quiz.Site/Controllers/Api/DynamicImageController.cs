namespace Quiz.Site.Controllers.Api;

using global::Quiz.Site.Models;
using global::Quiz.Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IO;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using StackExchange.Profiling.Internal;
using System.Threading.Tasks;

[ApiController]
[Route("api/img")]
public class DynamicImageController : ControllerBase
{
    private readonly IDynamicImageService _imageService;

    public DynamicImageController(IDynamicImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet]
    [Route("{hash}")]
    public async Task<FileResult> GetResultImage(string hash)
    {
        hash = new QuizResult()
        {
            Id = 1,
            DateCreated = DateTime.Now,
            MemberId = 2764,
            Name = "Matt Wise",
            QuizId = "2",
            Score = 10,
            Total = 10
        }.ToJson().ToUrlBase64();
    
        var converted = hash.FromUrlBase64();
        if (string.IsNullOrWhiteSpace(converted))
        {
            throw new FileNotFoundException();
        }

        var quizResult = JsonConvert.DeserializeObject<QuizResult>(converted);
        if(quizResult == null)
        {
            throw new FileNotFoundException();
        }

        using var image = await _imageService.CreateQuizResult(quizResult);

        var manager = new RecyclableMemoryStreamManager();
        var stream = manager.GetStream();
        image.SaveAsJpeg(stream);

        stream.Position = 0;        
        return File(stream, JpegFormat.Instance.DefaultMimeType);
    }
}
