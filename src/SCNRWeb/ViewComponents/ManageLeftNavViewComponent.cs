using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ON.Authentication;
using SubverseWeb.Models;
using System.Threading.Tasks;

namespace SubverseWeb.ViewComponents
{
    public class ManageLeftNavViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor context;
        private readonly ONUserHelper user;

        public ManageLeftNavViewComponent(IHttpContextAccessor context, ONUserHelper user)
        {
            this.context = context;
            this.user = user;
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View(new ManageLeftNavViewModel(context, user)));
        }
    }
}
