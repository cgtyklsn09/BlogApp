using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BlogContext>(options =>{options.UseSqlite(builder.Configuration["ConnectionStrings:sql_connection"]);

    // var version = new MySqlServerVersion(new Version(8,0,40));
    // options.UseMySql(connectionString, version);
});

builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {options.LoginPath = "/Users/Login";});

var app = builder.Build();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

SeedData.FillTestData(app);

app.MapControllerRoute(
    name: "post_details",
    pattern: "posts/details/{url}",
    defaults: new { controller = "Posts", action = "Details" }
);

app.MapControllerRoute(
    name: "posts_by_tag",
    pattern: "posts/tag/{url}",
    defaults: new { controller = "Posts", action = "Index" }
);

app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}"
);
app.Run();
