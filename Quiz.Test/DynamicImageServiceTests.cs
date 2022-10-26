namespace Quiz.Test;

using Microsoft.AspNetCore.Hosting;
using Moq;
using Quiz.Site.Models;
using Quiz.Site.Services;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

[TestFixture]
public class Tests
{
    private readonly string imageFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "images");

    private string outputDirectory;

    private string backgroundPath;
    
    private DynamicImageService imageService = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        backgroundPath = Path.Combine(imageFolder, "background.jpg");
        outputDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "output");
        Directory.CreateDirectory(outputDirectory);

        VerifyImageSharp.Initialize();

        FontCollection collection = new();
        collection.Add(Path.Combine(TestContext.CurrentContext.TestDirectory, "fonts", "OpenSans-Regular.ttf"));

        var fileSystem = Mock.Of<IPhysicalFileSystem>();
        Mock.Get(fileSystem)
            .Setup(f => f.OpenFile(It.IsAny<string>()))
            .Returns((string path) =>
            {
                if (path.Contains("background"))
                {
                    path = backgroundPath;
                }
                return System.IO.File.OpenRead(path);
            });

        var member = MockMemberWithAvatar();

        var accountService = Mock.Of<IAccountService>();
        Mock.Get(accountService)
            .Setup(a => a.GetMemberModelFromId(It.IsAny<int>()))
            .Returns(member);

        imageService = new DynamicImageService(collection, accountService, fileSystem, Mock.Of<IWebHostEnvironment>());
    }

    [OneTimeTearDown]
    public void TearDown() => Directory.Delete(outputDirectory, true);


    [Test]
    public async Task QuizResultImageTest()
    {
        using var image = await imageService.CreateQuizResult(new QuizResult
        {
            Score = 2,
            Total = 10,
            MemberId = 1
        });        

        var imagePath = Path.Combine(outputDirectory, $"{TestContext.CurrentContext.Test.Name.Replace(' ', '-')}.jpg");
        await image.SaveAsJpegAsync(imagePath);

        await VerifyFile(imagePath).EncodeAsPng();
    }

    

    private ContentModels.Member MockMemberWithAvatar()
    {
        var image = new Mock<ContentModels.Image>(Mock.Of<IPublishedContent>(), Mock.Of<IPublishedValueFallback>());
        image.Setup(x => x.UmbracoFile)
            .Returns(Mock.Of<ImageCropperValue>(icv => icv.Src == Path.Combine(imageFolder, "profile.jpg")));

        var member = new Mock<ContentModels.Member>(Mock.Of<IPublishedContent>(), Mock.Of<IPublishedValueFallback>());
        member.Setup(p => p.Avatar).Returns(new MediaWithCrops(image.Object, Mock.Of<IPublishedValueFallback>(), Mock.Of<ImageCropperValue>()));
        return member.Object;
    }
}