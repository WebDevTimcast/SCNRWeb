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
using ON.Fragments.Authentication;
using SCNRWeb.Models.CMS.News;
using SCNRWeb.Helper;

namespace SCNRWeb.Controllers
{
    [Route("rss")]
    public class RssController : Controller
    {
        private const int ITEMS_PER_PAGE = 30;

        private readonly ILogger logger;
        private readonly AssetService assetService;
        private readonly ContentService contentService;
        private readonly ContentUrlHelper cUrl;
        private readonly SettingsService settingsService;
        private readonly UserService userService;

        private const string MIME_TYPE = "application/rss+xml";

        public RssController(ILogger<RssController> logger, AssetService assetService, ContentService contentService, ContentUrlHelper cUrl, SettingsService settingsService, UserService userService)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.contentService = contentService;
            this.cUrl = cUrl;
            this.settingsService = settingsService;
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> All()
        {
            var items = await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = ITEMS_PER_PAGE,
            });

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("All", null, null, "https");
            using var ms = new MemoryStream();
            RssDocument.WriteRSS(doc, ms);

            return File(ms.ToArray(), MIME_TYPE);
        }

        [HttpGet("article")]
        [HttpGet("articles")]
        public async Task<IActionResult> Articles()
        {
            var items = await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = ITEMS_PER_PAGE,
                ContentType = ContentType.Written,
            });

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("Articles", null, null, "https");
            using var ms = new MemoryStream();
            RssDocument.WriteRSS(doc, ms);

            return File(ms.ToArray(), MIME_TYPE);
        }

        [HttpGet("/json/articles")]
        public async Task<IActionResult> JsonArticles()
        {
            var items = await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = 12,
                ContentType = ContentType.Written,
            });

            return Ok(new ExportModel(items, cUrl));
        }

        [AllowAnonymous]
        [HttpGet("/author/{authorId}/rss")]
        public async Task<IActionResult> AuthorPage(string authorId)
        {
            UserPublicRecord author;
            Guid authorGuid = authorId.ToGuid();
            author = await userService.GetUserPublic(authorGuid.ToString());

            if (author == null)
                return NotFound();

            var items = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = 0,
                AuthorId = authorId.ToString(),
            });
            if (items == null)
                return NotFound();

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("AuthorPage", null, new { authorId }, "https");
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
                PageSize = ITEMS_PER_PAGE,
                ContentType = ContentType.Video,
            });

            RssDocument doc = await CreateRssDoc(items);
            doc.Channel.AtomLink.InternalHref = Url.ActionLink("Videos", null, null, "https");
            using var ms = new MemoryStream();
            RssDocument.WriteRSS(doc, ms);

            return File(ms.ToArray(), MIME_TYPE);
        }

        private async Task<RssDocument> CreateRssDoc(GetAllContentResponse items)
        {
            var home = Url.ActionLink("Index", "Home", null, "https");

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
                    Link = new RssUrl(cUrl.GenerateFullContentUrl(item)),
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
                        Url = new RssUrl(cUrl.GenerateFullImageUrl(item)),
                        Type = image.Data.MimeType,
                        Length = image.Data.Data.Length,
                    };
                }

            }

            return list;
        }
    }
}
