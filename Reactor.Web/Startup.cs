using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reactor.Core;
using Reactor.Core.Domain.Chats;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Follows;
using Reactor.Core.Domain.Friends;
using Reactor.Core.Domain.Likes;
using Reactor.Core.Domain.Messages;
using Reactor.Core.Domain.Photos;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Domain.Users;
using Reactor.Core.Repository;
using Reactor.Data;
using Reactor.Data.EfContext;
using Reactor.Data.Repository;
using Reactor.Services.Chats;
using Reactor.Services.Follows;
using Reactor.Services.Friends;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Services.ViewRender;
using Reactor.Web.Hubs;
using Reactor.Web.Infrastructure.Extensions;
using Reactor.Web.Infrastructure.Helpers;
using Reactor.Web.Models.Chat;

namespace Reactor.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSignalR();

            services.AddNoTrailingSlash(options => options.RemoveTrailingSlash = true);

            services.AddDbContext<ReactorDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ReactorConnStr")));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ReactorDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ChatConnection>();

            services.AddScoped<IRepository<Friend>, FriendRepository>();
            services.AddScoped<IRepository<Post>, PostRepository>();
            services.AddScoped<IRepository<Photo>, PhotoRepository>();
            services.AddScoped<IRepository<Like>, LikeRepository>();
            services.AddScoped<IRepository<Comment>, CommentRepository>();
            services.AddScoped<IRepository<Follow>, FollowRepository>();
            services.AddScoped<IRepository<Message>, MessageRepository>();
            services.AddScoped<IRepository<Chat>, ChatRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<IFollowService, FollowService>();
            services.AddTransient<IChatService, ChatService>();

            services.AddTransient<CommonHelper>();

            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseNoTrailingSlash();

            app.UseStaticFiles();

            app.UseSignalR(configure => configure.MapHub<ChatHub>("/chathub"));

            app.UseMvc(routes => routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}