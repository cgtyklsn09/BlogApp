using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
    public class EfTagRepository : ITagRepository
    {
        private BlogContext context;
        public IQueryable<Tag> Tags => context.Tags;

        public EfTagRepository(BlogContext blogContext)
        {
            context = blogContext;
        }

        public void CreateTag(Tag tag)
        {
            context.Tags.Add(tag);
            context.SaveChanges();
        }
    }
}