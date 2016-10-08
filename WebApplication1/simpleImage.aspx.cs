using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var path = Path.Combine(new DirectoryInfo(Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path))).Parent.FullName, @"0001.dzipx");
            int width = 100;
            int.TryParse(Request["width"], out width);
            int height = 150;
            int.TryParse(Request["height"], out height);

            height = Convert.ToInt32(Request["height"]);
            var bitmap = GetImageFromDZPxCell.GetImageOfSize(path, new Size(width, height));
            Response.ContentType = "image/jpeg";
            bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);

        }
    }
}