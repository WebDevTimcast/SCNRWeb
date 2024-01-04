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
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View(new LeftNavModel(context)));
        }
    }
}
