using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class NewPosts : ViewComponent
    {
         private IPostRepository postRepository;

        public NewPosts(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await
                postRepository
                .Posts
                .OrderByDescending(p => p.PublishedOn)
                .Take(5)
                .ToListAsync()
            );
        }
    }
}