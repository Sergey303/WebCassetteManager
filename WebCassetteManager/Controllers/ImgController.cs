using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace WebCassetteManager.Controllers
{
    [RoutePrefix("img")]
    public class ImgController : Controller
    {

        [Route("dzipart/{id}/{level:int}/{x:int}_{y:int}.jpg")]
        public ActionResult GetImage(string id, int level, int x, int y)
        {
            //lock (locker)
            {
                // var path = Path.Combine(CurrentDirectory(), @"0001.dzipx");
                var bitmap = GetImageFromDZPxCell.GetImageFromDZPxCell.GetImageById(level, x, y, id);
                return new ImageResult(bitmap);
            }
        }

        [Route("example")]
        public ActionResult GetImage()
        {
            return new ContentResult()
            {
                Content =
                    GetImageFromDZPxCell.GetImageFromDZPxCell.GetOneImgHtml(
                        Path.Combine(ImgController.CurrentDirectory(),
                            @"0001.dzipx"), Request.Url.ToString().Replace("example", "")),
                ContentType = "text/html"
            };
        }

        [Route("dzi/{disc}/{*path}")]
        public ActionResult GetImage(string disc, string path)
        {
            return new ContentResult()
            {
                Content =
                    GetImageFromDZPxCell.GetImageFromDZPxCell.GetOneImgHtml(disc + ":" + path + ".dzipx",
                        Request.Url.ToString().Replace("path/" + disc + "/" + path, "")),
                ContentType = "text/html"
            };
        }

        public static string CurrentDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var dirName = new DirectoryInfo(Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path))).Parent.FullName;
            return dirName;
        }

   

        // GET: Img
        [Route("jpeg/{width:int}/{height:int}/{disc}/{*path}")]
        public ActionResult GetImage(int width, int height, string disc, string path)
        {
            var bitmap = GetImageFromDZPxCell.GetImageFromDZPxCell.GetImageOfSize(disc + ":" + path, width, height);
            return new ImageResult(bitmap);
        }

    }
    public class ImageResult : ActionResult
    {
        private readonly Bitmap bitmap;

        public ImageResult(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            bitmap.Save(context.HttpContext.Response.OutputStream, ImageFormat.Jpeg);
        }
    }
}
