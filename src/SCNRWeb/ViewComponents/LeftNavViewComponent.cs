using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCNRWeb.Models;

namespace SCNRWeb.ViewComponents
{
    public class LeftNavViewComponent : ViewComponent
    {
        private IHttpContextAccessor context;

        public LeftNavViewComponent(IHttpContextAccessor context)
        {
            this.context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new LeftNavModel(context));
        }
    }
}
