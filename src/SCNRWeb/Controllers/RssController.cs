using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ON.Authentication;
using ON.Crypto;
using SCNRWeb.Models;
using SCNRWeb.Models.Auth;
using SCNRWeb.Models.CMS;
using SCNRWeb.Services;
using X.Web.RSS;
using X.Web.RSS.Enumerators;
using X.Web.RSS.Structure.Validators;
using X.Web.RSS.Structure;
using System.IO;
using ON.Fragments.Content;
using ON.Fragments.Settings;
using ON.Fragments.Generic;

namespace SCNRWeb.Controllers
{
    [Route("rss")]
    public class RssController : Controller
    {
        private readonly ILogger logger;
        private readonly AssetService assetService;
        private readonly ContentService contentService;
        private readonly SettingsService settingsService;

        private const string MIME_TYPE = "application/rss+xml";

        public RssController(ILogger<RssController> logger, AssetService assetService, ContentService contentService, SettingsService settingsService)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.contentService = contentService;
            this.settingsService = settingsService;
        }

        [HttpGet("")]
        public async Task<IActionResult> All()
        {
            var items = await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = 30,
            });

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("All");
            using var ms = new MemoryStream();
            RssDocument.WriteRSS(doc, ms);

            return File(ms.ToArray(), MIME_TYPE);
        }

        [HttpGet("articles")]
        public async Task<IActionResult> Articles()
        {
            var items = await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = 30,
                ContentType = ContentType.Written,
            });

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("Articles");
            using var ms = new MemoryStream();
            RssDocument.WriteRSS(doc, ms);

            return File(ms.ToArray(), MIME_TYPE);
        }

        [HttpGet("videos")]
        public async Task<IActionResult> Videos()
        {
            var items = await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = 30,
                ContentType = ContentType.Video,
            });

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("Videos");
            using var ms = new MemoryStream();
            RssDocument.WriteRSS(doc, ms);

            return File(ms.ToArray(), MIME_TYPE);
        }

        private async Task<RssDocument> CreateRssDoc(GetAllContentResponse items)
        {
            var home = Url.ActionLink("Index", "Home");

            var doc = new RssDocument()
            {
                Channel = new RssChannel()
                {
                    AtomLink = new RssLink { InternalHref = home, Rel = Rel.self, Type = MIME_TYPE },
                    Category = "Newspapers",
                    Copyright = "Subverse Inc. (c) " + DateTime.UtcNow.Year,
                    Description = "SCNR is a community-owned news network by Subverse Inc. focused on investigative reporting, on-the-ground journalism and the free press. We produce a wide-spectrum of shows, podcasts, writing and documentaries on groundbreaking issues from diverse viewpoints.",
                    Image =
                        new RssImage
                        {
                            Description = "SCNR",
                            Height = 138,
                            Width = 582,
                            Link = new RssUrl(home),
                            Title = "SCNR",
                            Url = new RssUrl(home + "img/scnr-header.png")
                        },
                    Language = new CultureInfo("en-us"),
                    LastBuildDate = DateTime.UtcNow,
                    Link = new RssUrl(home),
                    PubDate = items?.Records?.FirstOrDefault()?.PublishOnUTC?.ToDateTime(),
                    Title = "SCNR",
                    TTL = 10,
                    Items = new List<RssItem>(),
                    Generator = null,
                    Docs = null,
                }
            };

            doc.Channel.Items.AddRange(await CreateItems(items));

            return doc;
        }

        private async Task<List<RssItem>> CreateItems(GetAllContentResponse items)
        {
            List<RssItem> list = new List<RssItem>();

            foreach (var item in items.Records)
            {
                var rss = new RssItem
                {
                    Title = item.Title,
                    Description = item.Description,
                    PubDate = item.PublishOnUTC.ToDateTime(),
                    Link = new RssUrl(Url.ActionLink("Get", "Content", new { id = item.ContentID, stub = item.URL })),
                    Guid = new RssGuid { IsPermaLink = false, Value = item.ContentID.ToString() },
                };

                list.Add(rss);

                var catId = item.CategoryIds.FirstOrDefault();
                CategoryRecord cat = null;
                if (!string.IsNullOrWhiteSpace(catId))
                {
                    cat = await settingsService.GetCategoryById(catId);
                }

                if (cat != null)
                    rss.Category = new() { Text = cat.DisplayName };


                var image = await assetService.GetImage(item.FeaturedImageAssetID.ToGuid());
                if (image != null)
                {
                    rss.Enclosure = new()
                    {
                        Url = new RssUrl(Url.ActionLink("Get", "Asset", new { id = image.AssetID })),
                        Type = image.Data.MimeType,
                        Length = image.Data.Data.Length,
                    };
                }

            }

            return list;
        }
    }
}
