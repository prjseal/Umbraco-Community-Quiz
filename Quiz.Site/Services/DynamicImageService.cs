namespace Quiz.Site.Services;

using global::Quiz.Site.Extensions;
using global::Quiz.Site.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Linq;
using Umbraco.Cms.Core.IO;

public sealed class DynamicImageService : IDynamicImageService
{
    private readonly IFontCollection _fontCollection;

    private readonly IAccountService _accountService;

    private readonly IPhysicalFileSystem _fileSystem;

    private readonly IWebHostEnvironment _hostEnvironment;

    private readonly Font _smallFont;

    private readonly Font _largeFont;

    private const string scoreImagePath = "/assets/img/social/score-background.jpg";

    private const string preText = "I scored";

    private const string communityText = "on this weeks\nUmbraco\nCommunity\nQuiz";

    public DynamicImageService(
        IFontCollection fontCollection,
        IAccountService accountService,
        IPhysicalFileSystem fileSystem,
        IWebHostEnvironment hostEnvironment)
    {
        if (fontCollection.Families?.Any() != true)
        {
            throw new ArgumentOutOfRangeException(nameof(fontCollection), "No fonts loaded");
        }

        _fontCollection = fontCollection;
        _accountService = accountService;
        if (_fontCollection.TryGet("Open Sans", out var family) == false)
        {
            family = _fontCollection.Families.FirstOrDefault();
        }

        _smallFont = family.CreateFont(40, FontStyle.Bold);
        _largeFont = family.CreateFont(110, FontStyle.Bold);
        _fileSystem = fileSystem;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<Image> CreateQuizResult(QuizResult result, CancellationToken cancellationToken = default)
    {
        using var source = _fileSystem.OpenFile(_hostEnvironment.MapPathWebRoot(scoreImagePath));

        var image = await Image.LoadAsync(source, cancellationToken);

        var memeber = _accountService.GetMemberModelFromId(result.MemberId);
        if (memeber?.Avatar?.Content is Umbraco.Cms.Web.Common.PublishedModels.Image avatar && !string.IsNullOrEmpty(avatar.UmbracoFile?.Src))
        {
            await AddAvataToImage(image, avatar.UmbracoFile.Src);

        }

        WriteResultText(image, result);
        return image;
    }

    private async Task AddAvataToImage(Image image, string src)
    {
        using var avatar = _fileSystem.OpenFile(_hostEnvironment.MapPathWebRoot(src));

        var avatarImage = await Image.LoadAsync(avatar);

        var roundedAvatar = avatarImage.Clone(x =>
        x.ConvertToAvatar(new Size(100, 100), 50, new Rgba32(63, 71, 204, 1)));

        image.Mutate(x => x.DrawImage(roundedAvatar, new Point(390, 10), 1));
    }

    private void WriteResultText(Image image, QuizResult result)
    {
        var lines = new string[]
        {
            preText,
            $"{result.Score}/{result.Total}",
            communityText
        };

        var currentFont = _smallFont;
        var origin = new PointF(20, 20);

        image.Mutate(x => {
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                //TODO:Need to check against long usernames once the background and positions is done
                //You can use the wrap settings in TextOptions so it wraps instead.
                var options = new TextOptions(currentFont)
                {
                    Origin = origin
                };

                x.DrawText(options, line, Color.White);
                
                //Or using this information, you could loader a new options with a smaller font size.
                var size = TextMeasurer.Measure(line, options);

                origin = new PointF(20, origin.Y + size.Height);
                currentFont = i % 2 != 0 ? _smallFont : _largeFont;
            }
        });
    }
}
