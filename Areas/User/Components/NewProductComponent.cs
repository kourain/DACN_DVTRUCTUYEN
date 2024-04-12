using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "NewProduct")]
    public class NewProductComponent : ViewComponent
    {
        private readonly DataContext _context;
        public NewProductComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}
