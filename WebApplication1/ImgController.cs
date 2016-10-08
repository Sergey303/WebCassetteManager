using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web.Mvc;


[RoutePrefix("dzi")]
    public class ImgController : Controller
    {
        public static object locker=new object();
        
        [Route("{id}/{level:int}/{x:int}_{y:int}.jpg")]
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
                GetImageFromDZPxCell.GetImageFromDZPxCell.GetOneImgHtml(Path.Combine(ImgController.CurrentDirectory(),
                    @"0001.dzipx"), Request.Url.ToString().Replace("example", "")),
            ContentType = "text/html"
        };
    }
    [Route("path/{disc}/{*path}")]
    public ActionResult GetImage(string disc, string path)
    {
        return new ContentResult()
        {
            Content =
                GetImageFromDZPxCell.GetImageFromDZPxCell.GetOneImgHtml(disc + ":" + path + ".dzipx", Request.Url.ToString().Replace("path/" + disc + "/" + path, "")),
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
