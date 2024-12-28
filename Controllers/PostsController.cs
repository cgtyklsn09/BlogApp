using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private IPostRepository postRepository;
        private ICommentRepository commentRepository;
        private ITagRepository tagRepository;
        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository)
        {
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
            this.tagRepository = tagRepository;
        }

        public async Task<IActionResult> Index(string? url)
        {
            var claims =User.Claims;

            var posts = postRepository.Posts.Where(i => i.IsActive);

            if (!string.IsNullOrEmpty(url))
                posts = posts.Where(x => x.Tags.Any(y => y.Url == url));

            return View(new PostsViewModel{Posts = await posts.ToListAsync()});
        }

        public async Task<IActionResult> Details(string url)
        {
            var post = await postRepository
            .Posts
            .Include(x => x.User)
            .Include(x => x.Tags)
            .Include(x => x.Comments)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(p => p.Url == url);

            return View(post);
        }

        [HttpPost]
        public JsonResult AddComment(int postId, string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);
            var name = User.FindFirstValue(ClaimTypes.GivenName);
            var comment = new Comment{
                Text = text,
                PublishedOn = DateTime.Now,
                PostId = postId,
                UserId = int.Parse(userId ?? "")
            };

            commentRepository.CreateComment(comment);

            return Json(new {
                userName, 
                name,
                text,
                comment.PublishedOn,
                avatar
            });
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(PostCreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                postRepository.CreatePost(
                    new Post {
                        Title = model.Title,
                        Content = model.Content,
                        Url = model.Url,
                        UserId = int.Parse(userId ?? ""),
                        PublishedOn = DateTime.Now,
                        Image = "1.jpg",
                        IsActive = false
                    }
                );

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = postRepository.Posts;

            if(string.IsNullOrEmpty(role))
                posts = posts.Where(x => x.UserId == userId);
            
            return View(await posts.ToListAsync());
        }

        public IActionResult Edit(int? id)
        {
            if(id == null)
                return NotFound();
            
            var post = postRepository.Posts.Include(x => x.Tags).FirstOrDefault(x => x.PostId == id);
            if(post == null)
                return NotFound();
            
            ViewBag.Tags = tagRepository.Tags.ToList();
            
            return View(new PostCreateViewModel(){
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Url = post.Url,
                IsActive = post.IsActive,
                Tags = post.Tags
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(PostCreateViewModel model, int[] tagIds)
        {
            if(ModelState.IsValid)
            {
                var post = new Post
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Content = model.Content,
                    Description = model.Description,
                    Url = model.Url,
                };

                if(User.FindFirstValue(ClaimTypes.Role) == "admin")
                    post.IsActive = model.IsActive;
                
                postRepository.EditPost(post, tagIds);
                return RedirectToAction("List");
            }

            ViewBag.Tags = tagRepository.Tags.ToList();
            return View(model);
        }
    }
}