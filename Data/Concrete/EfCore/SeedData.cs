using Microsoft.EntityFrameworkCore;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
    public static class SeedData
    {
        public static void FillTestData(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if(context != null)
            {
                if(context.Database.GetPendingMigrations().Any()) //uygulanmamış migration varsa
                {
                    context.Database.Migrate();
                }

                if(!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Tag{Text = "web programlama", Url = "web-programlama", TagColor = TagColors.primary},
                        new Tag{Text = "frontend", Url = "frontend", TagColor = TagColors.danger},
                        new Tag{Text = "backend", Url = "backend", TagColor = TagColors.warning},
                        new Tag{Text = "fullstack", Url = "fullstack", TagColor = TagColors.success},
                        new Tag{Text = "veri bilimi", Url = "veri-bilimi", TagColor = TagColors.secondary}
                    );

                    context.SaveChanges();
                }

                if(!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User{UserName = "cagataykalsan", Name = "Çağatay", Email = "cagataykalsan@gmail.com", Password = "246800", Image = "p1.png"},
                        new User{UserName = "sevgikalsan", Name = "Sevgi", Email = "sevgikalsan@gmail.com", Password = "147258", Image = "p2.png"}
                    );

                    context.SaveChanges();
                }

                if(!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post{
                        Title = "Asp.net core", 
                        Url = "asp-net-core",
                        Description = "Asp.net core dersleri",
                        Content = "Asp.net core dersleri", 
                        IsActive = true, 
                        PublishedOn = DateTime.Now.AddDays(-10), 
                        Tags = context.Tags.Take(3).ToList(),
                        Image = "1.jpg",
                        UserId = 1,
                        Comments = new List<Comment>
                        {
                            new Comment{Text = "iyi bir kurs", 
                            PublishedOn = DateTime.Now.AddDays(-5), 
                            UserId = 2}
                        }
                        },
                        new Post{
                        Title = "React native", 
                        Url = "react-native",
                        Description = "React native dersleri",
                        Content = "React native dersleri", 
                        IsActive = true, 
                        PublishedOn = DateTime.Now.AddDays(-20), 
                        Tags = context.Tags.Take(2).ToList(),
                        Image = "2.jpg",
                        UserId = 1
                        },
                        new Post{
                        Title = "Vue.js", 
                        Url = "vue-js",
                        Description = "Vue.js dersleri",
                        Content = "Vue.js dersleri", 
                        IsActive = true, 
                        PublishedOn = DateTime.Now.AddDays(-15), 
                        Tags = context.Tags.Take(4).ToList(),
                        Image = "3.jpg",
                        UserId = 2
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}