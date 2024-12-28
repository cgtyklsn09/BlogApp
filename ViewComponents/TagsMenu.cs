using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class TagsMenu : ViewComponent
    {
        private ITagRepository tagRepository;

        public TagsMenu(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await tagRepository.Tags.ToListAsync());
        }
    }
}