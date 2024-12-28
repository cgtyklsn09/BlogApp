using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
    public class EfUserRepository : IUserRepository
    {
        private BlogContext context;

        public IQueryable<User> Users => context.Users;

        public EfUserRepository(BlogContext blogContext)
        {
            context = blogContext;
        }

        User IUserRepository.CreateUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();

            return user;
        }
    }
}