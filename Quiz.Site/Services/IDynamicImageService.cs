namespace Quiz.Site.Services;

using SixLabors.ImageSharp;
using System.Threading;
using System.Threading.Tasks;

public interface IDynamicImageService
{
    Task<Image> CreateQuizResult(Models.QuizResult result, CancellationToken cancellationToken = default);
}