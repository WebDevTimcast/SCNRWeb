using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCNRWeb.Models;

namespace SCNRWeb.ViewComponents
{
    public class TopNavViewComponent : ViewComponent
    {
        private IHttpContextAccessor context;

        public TopNavViewComponent(IHttpContextAccessor context)
        {
            this.context = context;
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View(new TopNavModel(context)));
        }
    }
}
