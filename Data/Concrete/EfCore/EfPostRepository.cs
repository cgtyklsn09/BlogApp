using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EfCore
{
    public class EfPostRepository : IPostRepository
    {
        private BlogContext context;
        public IQueryable<Post> Posts => context.Posts;

        public EfPostRepository(BlogContext blogContext)
        {
            context = blogContext;
        }

        public void CreatePost(Post post)
        {
            context.Posts.Add(post);
            context.SaveChanges();
        }

        public async void EditPost(Post post)
        {
            var entity = await context.Posts.FirstOrDefaultAsync(p => p.PostId == post.PostId);
            if(entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.Content = post.Content;
                entity.Url = post.Url;
                entity.IsActive = post.IsActive;

                await context.SaveChangesAsync();
            }
        }

        public async void EditPost(Post post, int[] tagIds)
        {
            var entity = await context.Posts.Include(x=>x.Tags).FirstOrDefaultAsync(p => p.PostId == post.PostId);
            if(entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.Content = post.Content;
                entity.Url = post.Url;
                entity.IsActive = post.IsActive;
                entity.Tags = context.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToList();

                await context.SaveChangesAsync();
            }
        }
    }
}