using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using
using Quiz.Site.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Quiz.Site.Services;
public class BadgeService : IBadgeService
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IMemberService _memberService;

    public BadgeService(IUmbracoContextFactory umbracoContextFactory, IMemberService memberService)
    {
        _umbracoContextFactory = umbracoContextFactory;
        _memberService = memberService;
    }

    public bool AddBadgeToMember(int memberId, IPublishedContent contentItem)
    {
        using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
        {
            var udi = contentItem.GetUdiObject().ToString();

            var member = _memberService.GetById(memberId);

            var badgeUdis = member.GetValue<IEnumerable<string>>("badges").ToList();
            
            if(badgeUdis != null && badgeUdis.Any() && !badgeUdis.Contains(udi))
            {
                badgeUdis.Add(udi);
            }

            var galleryValue = member.GetValue<string>("gallery");

            if (!string.IsNullOrWhiteSpace(galleryValue))
            {
                JArray galleryArray = JsonConvert.DeserializeObject<JArray>(galleryValue);

                var sortOrderArray =
                model.GallerySortOrder.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x)).ToArray();

                var sortedArray = new JArray();

                var numberOfItems = galleryArray.Count;
                foreach (var index in sortOrderArray)
                {
                    if (index < numberOfItems)
                    {
                        sortedArray.Add(galleryArray[index]);
                    }
                }

                var json = JsonConvert.SerializeObject(sortedArray);

                member.SetValue("gallery", json);
                galleryValue = json;
            }

            if (model.Gallery != null && model.Gallery.Any())
            {
                JArray galleryArray = null;
                if (!string.IsNullOrWhiteSpace(galleryValue))
                {
                    galleryArray = JsonConvert.DeserializeObject<JArray>(galleryValue);
                }
                else
                {
                    galleryArray = new JArray();
                }

                foreach (var item in model.Gallery.Where(x => x != null))
                {
                    var mediaKey = _mediaUploadService.CreateMediaItemFromFileUpload(item, 1126, "Image", returnUdi: false);

                    if (!string.IsNullOrWhiteSpace(mediaKey))
                    {
                        JObject galleryItem = new JObject();
                        galleryItem.Add("key", Guid.NewGuid().ToString());
                        galleryItem.Add("mediaKey", mediaKey);
                        galleryItem.Add("crops", null);
                        galleryItem.Add("focalPoint", null);

                        galleryArray.Add(galleryItem);
                    }
                }

                member.SetValue("gallery", galleryArray);
            }

        }

        return true;
    }
}
