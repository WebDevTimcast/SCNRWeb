using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ON.Fragments.Content;
using SCNRWeb.Helper;
using SCNRWeb.Services;
using System.Linq;
using System.Threading.Tasks;
using SCNRWeb.Models.CMS.News;
using SCNRWeb.Controllers;

namespace SCNRWeb.ViewComponents
{
    public class PopularNewsViewComponent : ViewComponent
    {
        private readonly ContentService contentService;

        public PopularNewsViewComponent(ContentService contentService)
        {
            this.contentService = contentService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PopularNewsModel();
            var res = await contentService.GetAll(new()
            {
                PageSize = 6,
                PageOffset = 0,
                ContentType = ContentType.Written,
            });

            HttpContext.Items.TryGetValue(ContentController.CURRENT_CONTENT_ID, out var currentId);

            model.Records = res.Records.Where(r => r.ContentID != (currentId as string)).Take(5).ToList();
            foreach (var record in model.Records)
                record.Title = record.Title.TruncatePretty(53);

            return View(model);
        }
    }
}
