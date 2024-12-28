using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
    public class EfCommentRepository : ICommentRepository
    {
        private BlogContext context;
        public IQueryable<Comment> Comments => context.Comments;

        public EfCommentRepository(BlogContext blogContext)
        {
            context = blogContext;
        }

        public void CreateComment(Comment comment)
        {
            context.Comments.Add(comment);
            context.SaveChanges();
        }
    }
}