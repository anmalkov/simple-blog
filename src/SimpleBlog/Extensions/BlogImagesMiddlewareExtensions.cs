using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Reflection;

namespace SimpleBlog.Extensions
{
    public static class BlogImagesMiddlewareExtensions
    {
        public static IApplicationBuilder UseBlogImages(this IApplicationBuilder builder)
        {
            var blogImagesDirectory = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "images");
            if (!Directory.Exists(blogImagesDirectory))
            {
                Directory.CreateDirectory(blogImagesDirectory);
            }
            return builder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(blogImagesDirectory),
                RequestPath = "/blogimages"
            });
        }
    }
}
