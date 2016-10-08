using System;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using WebCassetteManager.Models;

namespace WebCassetteManager.Controllers
{
    [RoutePrefix("uri")]
    public class UriController : Controller
    {
        [Route("doc")]
        public ActionResult GetDocument(string u)
        {
            string ext;
            string docId;
            var cassetteModel = CassetteModel(u, out ext, out docId);
                    var fullName = Path.Combine(cassetteModel.Path, "documents\\deepzoom\\", docId);
            switch (ext)
            {
                case ".tif":
                case ".jpg":
                    var path = fullName + ".dzipx";
                    if (!System.IO.File.Exists(path))
                    {
                        //todo
                        var sarcPath= fullName + ".sarc2";
                        //if(System.IO.File.Exists(sarcPath))
                        var originalPath = GetOriginalPath(cassetteModel, docId, ext);
                        //if (System.IO.File.Exists(originalPath)) GetImageFromDZPxCell.GetImageFromDZPxCell.
                    }
                    var seadragonhtml = GetImageFromDZPxCell.GetImageFromDZPxCell.GetOneImgHtml(path, Request.Url.ToString().Replace("uri/doc", "img"));
                    return Content(seadragonhtml, "text/html");

                case ".unknown":
                    return File(fullName + ext, "text/text");
                case ".html":
                    return File(fullName + ext, "text/html");
                default:
                    throw new Exception(ext);
            }
        }
        [Route("preview")]
        public ActionResult GetPreview(string u, int width)
        {
            string ext;
            string docId;
            var cassetteModel = CassetteModel(u, out ext, out docId);
            var fullName = Path.Combine(cassetteModel.Path, "documents\\deepzoom\\", docId);
            switch (ext)
            {
                case ".tif":
                case ".jpg":
                    var path = fullName + ".dzipx";
                    if (!System.IO.File.Exists(path))
                    {
                        //todo
                        var sarcPath = fullName + ".sarc2";
                        //if(System.IO.File.Exists(sarcPath))
                        var originalPath = GetOriginalPath(cassetteModel, docId, ext);
                        //if (System.IO.File.Exists(originalPath)) GetImageFromDZPxCell.GetImageFromDZPxCell.
                    }
                    var bitmap = GetImageFromDZPxCell.GetImageFromDZPxCell.GetImageOfSize(path, width);
                    return new ImageResult(bitmap);

                case ".unknown":
                    return Content("unknown doc icon");
                case ".html":
                    return Content("html doc icon");
                default:
                    throw new Exception(ext);
            }
        }

        private CassetteModel CassetteModel(string u, out string ext, out string docId)
        {
            var @i = u.IndexOf('@');
            var cassetteName = u.Substring(7, @i - 7);
            var cassetteModel = CasssettesBD.CasssettesBd.Cassettes.Find(model => model.Name == cassetteName);
            if (cassetteModel == null)
                OnException(new ExceptionContext(ControllerContext,
                    new Exception("cassette not connected " + cassetteName + " uri=" + u)));
            var docIdext = u.Substring(@i + 12);
            var doti = docIdext.IndexOf('.');

            ext = docIdext.Substring(doti);
            docId = docIdext.Substring(0, docIdext.Length - doti);
            if (docId.Length == 15) docId = docId.Substring(5);
            return cassetteModel;
        }

        [Route("original")]
        public ActionResult GetOriginal(string u)
        {
            string ext;
            string docId;
            var cassetteModel = CassetteModel(u, out ext, out docId);
           var origianalPath= GetOriginalPath(cassetteModel, docId, ext);
            switch (ext)
            {
                case ".tif":
                case ".jpg":
                    return new ImageResult(new Bitmap(origianalPath));
                case ".unknown":
                    return File(origianalPath, "text/text");
                case ".html":
                  return  File(origianalPath, "text/html");
                default:
                    throw new Exception(ext);
            }
        }

        private static string GetOriginalPath(CassetteModel cassetteModel, string docId, string ext)
        {
           return Path.Combine(cassetteModel.Path, "originals\\", docId) + ext;
        }

        #region Deprecated

        [Route("jpeg/iiss_{cassetteName}@iis.nsk.su/0001/{dir}/{name}/{width:int}/{height:int}")]
        public ActionResult GetImage(string cassetteName, string dir, string name, int width, int height)
        {
            var cassetteModel = CasssettesBD.CasssettesBd.Cassettes.Find(model => model.Name == cassetteName);
            var path = Path.Combine(cassetteModel.Path, "documents\\deepzoom", dir, name) + ".dzipx";

            var disc = Path.GetPathRoot(path);
            return new ImgController().GetImage(width, height, disc.Substring(0, 1), path.Substring(disc.Length));
        }

        [Route("dzi/iiss_{cassetteName}@iis.nsk.su/0001/{dir}/{name}/")]
        public ActionResult GetDZI(string cassetteName, string dir, string name)
        {
            var cassetteModel = CasssettesBD.CasssettesBd.Cassettes.Find(model => model.Name == cassetteName);
            var path = Path.Combine(cassetteModel.Path, "documents\\deepzoom", dir, name) + ".dzipx";

            var imgUrl =
                Request.Url.ToString()
                    .Replace("uri/dzi/iiss_" + cassetteName + "@iis.nsk.su/0001/" + dir + "/" + name, "img/dzipart/");

            return new ContentResult()
            {
                Content = GetImageFromDZPxCell.GetImageFromDZPxCell.GetOneImgHtml(path, imgUrl),
                ContentType = "text/html"
            };
        }

        #endregion


    }
}
