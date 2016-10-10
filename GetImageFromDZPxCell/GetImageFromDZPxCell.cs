using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.DeepZoomTools;
using PolarDB;

namespace GetImageFromDZPxCell
{
    public static class GetImageFromDZPxCell
    {
        private static void Main(string[] args)
        {
            //Create("../../0174.sarc2");
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //GetImageOfSize("../../0174.sarc2", new Size(100, 100));
            //timer.Stop();
            //Console.WriteLine(timer.ElapsedMilliseconds);
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var path =
                Path.Combine(
                    new DirectoryInfo(Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path))).Parent.Parent.FullName,
                    @"0174.bmp");
            CreateFromBmp(path);
        }

        public static Bitmap GetImageOfSize(string path, int width = -1, int height = -1)
        {
            lock (locker)
            {
                PxCell cell = null;
                string id;
                if (!path2id.TryGetValue(path, out id))
                {
                    id = Guid.NewGuid().ToString().Replace('-', '0');
                    path2id.Add(path, id);
                    id2cell.Add(id,
                        cell = new PxCell(
                            new PTypeRecord(new NamedType("width", new PType(PTypeEnumeration.integer)),
                                new NamedType("height", new PType(PTypeEnumeration.integer)),
                                new NamedType("images",
                                    new PTypeSequence( //by level
                                        new PTypeSequence( //by x
                                            new PTypeSequence( //by y
                                                new PTypeSequence(new PType(PTypeEnumeration.@byte))))))),
                            path));
                }
                else
                {
                    cell = id2cell[id];
                }

                Size maxSize = new Size((int) cell.Root.Field(0).Get(), (int) cell.Root.Field(1).Get());
                int level = (int) cell.Root.Field(2).Count() - 1;
                float aspectRatio = maxSize.Width * 1f / maxSize.Height;

                
                if (width == -1 && height == -1)
                {
                    width = maxSize.Width;
                    height = maxSize.Height;
                }
                else if (width == -1)
                {
                    width = (int) (height*aspectRatio);
                }
                else if (height == -1)
                {
                    height = (int) (width * 1f / aspectRatio);
                }
                int findedWidth;
                var levelw = GetMaxLevel(width, maxSize.Width, level, out findedWidth);
                int findedHeight;
                var levelh = GetMaxLevel(height, maxSize.Height, level, out findedHeight);


                float changeWidth = 1f, changeHeight = 1f;
                if (levelh > levelw)
                {
                    level = levelh;
                    changeHeight = height * 1f / findedHeight;
                    changeWidth = width * 1f/(findedHeight * aspectRatio);
                }
                else
                {
                    level = levelw;
                    changeHeight = height * aspectRatio / findedWidth;
                    changeWidth = width * 1f / findedWidth;
                }
                Bitmap outputImage = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(outputImage))
                {
                    //      graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                        int x = 0;
                        var xc = cell.Root.Field(2).Element(level).Count();
                        for (int i = 0; i < xc; i++) //x
                        {
                            var yc = cell.Root.Field(2).Element(level).Element(i).Count();
                            int y = 0;
                            int w = 0;
                            for (int j = 0; j < yc; j++) //y
                            {
                                var img = GetImage(level, i, j, cell);
                                w = (int) (img.Size.Width*changeWidth);
                                var h = (int) (img.Height*changeHeight);
                                graphics.DrawImage(img, new Rectangle(new Point(x, y), new Size(w, h)),
                                    0, 0, img.Size.Width, img.Size.Height, GraphicsUnit.Pixel, wrapMode);
                                y += h;
                            }
                            x += w;
                        }
                    }
                }
                return outputImage;
                //Stopwatch timer=new Stopwatch();
                //timer.Start();
                //outputImage.Save("../../0174.jpg", ImageFormat.Jpeg);
                //timer.Stop();
                //Console.WriteLine(timer.ElapsedMilliseconds);
            }
        }

        private static int GetMaxLevel(int needwidth, int currnetWidth, int level, out int finded)
        {
            needwidth += needwidth;
            while (needwidth <= currnetWidth) // need <= currnet/2
            {
                level--;
                currnetWidth = currnetWidth/2;
            }
            finded = currnetWidth;
            return level;
        }

        public static Bitmap GetImage(int level, int x, int y, PxCell cell)
        {
            return
                new Bitmap(new MemoryStream(((object[]) cell.Root.Field(2).Element(level).Element(x).Element(y).Get())
                    .Cast<byte>().ToArray()));
        }


        public static Dictionary<string, PxCell> id2cell = new Dictionary<string, PxCell>();
        public static Dictionary<string, string> path2id = new Dictionary<string, string>();
        private static object locker = new object();

        public static Bitmap GetImageByPath(int level, int x, int y, string path)
        {
            lock (locker)
            {
                PxCell cell = null;
                string id;
                if (!path2id.TryGetValue(path, out id))
                {
                    id = Guid.NewGuid().ToString().Replace('-', '0');
                    path2id.Add(path, id);
                    id2cell.Add(id,
                        cell = new PxCell(
                            new PTypeRecord(new NamedType("width", new PType(PTypeEnumeration.integer)),
                                new NamedType("height", new PType(PTypeEnumeration.integer)),
                                new NamedType("images",
                                    new PTypeSequence( //by level
                                        new PTypeSequence( //by x
                                            new PTypeSequence( //by y
                                                new PTypeSequence(new PType(PTypeEnumeration.@byte))))))),
                            path));
                }
                return GetImage(level, x, y, cell);
            }
        }

        public static Bitmap GetImageById(int level, int x, int y, string id)
        {
            lock (locker)
            {
                PxCell cell = null;
                if (!id2cell.TryGetValue(id, out cell)) return null;
                return GetImage(level, x, y, cell);
            }
        }

        //public static Size GetSize(string cellPath)
        //{
        //    lock (locker)
        //    {
        //        PxCell cell;
        //        if (!cells.TryGetValue(cellPath, out cell))
        //            cells.Add(cellPath,
        //                cell = new PxCell(
        //                    new PTypeRecord(new NamedType("width", new PType(PTypeEnumeration.integer)),
        //                        new NamedType("height", new PType(PTypeEnumeration.integer)),
        //                        new NamedType("images",
        //                            new PTypeSequence( //by level
        //                                new PTypeSequence( //by x
        //                                    new PTypeSequence( //by y
        //                                        new PTypeSequence(new PType(PTypeEnumeration.@byte))))))),
        //                    cellPath));
        //        return new Size((int) cell.Root.Field(0).Get(), (int) cell.Root.Field(1).Get());
        //    }

        //}
        public static void CreatefromSarc2(string sarc2path)
        {

            Stream stream = new FileStream(sarc2path, FileMode.Open, FileAccess.Read);

            long offset = 0L;
            for (int i = 0; i < 16; i++)
            {
                int b = stream.ReadByte();
                if (b != 0) offset = offset*10 + (b - '0');
            }

            byte[] catalog_bytes = new byte[offset - 16];

            stream.Read(catalog_bytes, 0, (int) (offset - 16));
            int width = 0;
            int height = 0;

            var levels = XElement.Load(new XmlTextReader(new MemoryStream(catalog_bytes))).Elements("file")
                .Select(xfile =>
                {
                    string r_path = xfile.Element("path").Value;
                    var regex = new Regex(@"(?<level>[0-9]+)/(?<x>[0-9]+)_(?<y>[0-9]+)\.jpg$");

                    var match = regex.Match(r_path);
                    if (!match.Success)
                        if (Path.GetExtension(r_path) == ".xml")
                        {
                            stream.Seek(long.Parse(xfile.Element("start").Value) + offset, SeekOrigin.Begin);
                            var dataxml = new byte[long.Parse(xfile.Element("length").Value)];
                            stream.Read(dataxml, 0, dataxml.Length);
                            XElement xDzi = XElement.Load(XmlReader.Create(new MemoryStream(dataxml)));
                            var xElement =
                                xDzi.Element(XNamespace.Get("http://schemas.microsoft.com/deepzoom/2009") + "Size");

                            width = int.Parse(xElement.Attribute("Width").Value);
                            height = int.Parse(xElement.Attribute("Height").Value);

                            return null;
                        }
                        else throw new Exception();
                    int level = Convert.ToInt32(match.Groups["level"].Value);
                    int x = Convert.ToInt32(match.Groups["x"].Value);
                    int y = Convert.ToInt32(match.Groups["y"].Value);
                    stream.Seek(long.Parse(xfile.Element("start").Value) + offset, SeekOrigin.Begin);
                    var data = new byte[long.Parse(xfile.Element("length").Value)];
                    stream.Read(data, 0, data.Length);
                    return new {level, x, y, data = data.Cast<object>().ToArray()};
                })
                .Where(arg => arg != null)
                .GroupBy(arg => arg.level)
                .OrderBy(level => level.Key)
                .Select(gl =>
                    gl.GroupBy(arg => arg.x)
                        .OrderBy(xs => xs.Key)
                        .Select(gx =>
                            gx.OrderBy(arg => arg.y)
                                .Select(arg => arg.data)
                                .ToArray())
                        .ToArray())
                .ToArray();
            stream.Close();

            if (width == 0 || height == 0) throw new Exception();

            var cell =
                new PxCell(
                    new PTypeRecord(new NamedType("width", new PType(PTypeEnumeration.integer)),
                        new NamedType("height", new PType(PTypeEnumeration.integer)),
                        new NamedType("images",
                            new PTypeSequence( //by level
                                new PTypeSequence( //by x
                                    new PTypeSequence( //by y
                                        new PTypeSequence(new PType(PTypeEnumeration.@byte))))))),
                    Path.ChangeExtension(sarc2path, ".dz_px"), false);


            cell.Fill(new object[] {width, height, levels});
        }

        public static void CreateFromBmp(string bmpPath)
        {
            var cell =
                new PxCell(
                    new PTypeRecord(new NamedType("width", new PType(PTypeEnumeration.integer)),
                        new NamedType("height", new PType(PTypeEnumeration.integer)),
                        new NamedType("images",
                            new PTypeSequence( //by level
                                new PTypeSequence( //by x
                                    new PTypeSequence( //by y
                                        new PTypeSequence(new PType(PTypeEnumeration.@byte))))))),
                    Path.ChangeExtension(bmpPath, ".dzipx"), false);
            ImageCreator icreator = new ImageCreator {ImageQuality = 0.8};
            var filePath = Path.ChangeExtension(bmpPath, ".xml");
            icreator.Create(bmpPath, filePath);
            var dirPath = filePath.Replace(".xml", "") + "_files";
            using (var img = new Bitmap(bmpPath))
            {
                cell.Fill(new object[]
                {
                    img.Size.Width, img.Size.Height,
                    Directory.EnumerateDirectories(dirPath)
                        .Select(level => Directory.EnumerateFiles(level)
                            .Select(x_yjpg =>
                            {
                                var xy = Path.GetFileNameWithoutExtension(x_yjpg).Split('_');
                                return
                                    new
                                    {
                                        data = File.ReadAllBytes(x_yjpg).Cast<object>().ToArray(),
                                        x = xy[0],
                                        y = xy[1]
                                    };
                            })
                            .GroupBy(arg => arg.x)
                            .OrderBy(xs => xs.Key)
                            .Select(gx =>
                                gx.OrderBy(arg => arg.y)
                                    .Select(arg => arg.data)
                                    .ToArray())
                            .ToArray())
                        .ToArray()
                });
            }

            File.Delete(filePath);
            Directory.Delete(dirPath, true);
        }

        // private static long nextId = 0;

        public static string GetOneImgHtml(string cellPath, string url)
        {
            lock (locker)
            {
                PxCell cell = null;
                string id;
                if (!path2id.TryGetValue(cellPath, out id))
                {
                    if (!File.Exists(cellPath)) return "File not exists " + cellPath;
                    cell =
                        new PxCell(
                            new PTypeRecord(new NamedType("width", new PType(PTypeEnumeration.integer)),
                                new NamedType("height", new PType(PTypeEnumeration.integer)),
                                new NamedType("images",
                                    new PTypeSequence( //by level
                                        new PTypeSequence( //by x
                                            new PTypeSequence( //by y
                                                new PTypeSequence(new PType(PTypeEnumeration.@byte))))))),
                            cellPath);
                    id = Guid.NewGuid().ToString().Replace('-', '0');
                    path2id.Add(cellPath, id);
                    id2cell.Add(id, cell);
                }
                else
                {
                    cell = id2cell[id];
                }
                if (cell == null) return "cell is null "+cellPath+" id:"+id;
                    return @"    <div id='openseadragon" + id + @"' >

</div>
  <script src='/openseadragon/openseadragon.min.js'></script>
   <script type='text/javascript'>
        var viewer = OpenSeadragon({
            id: 'openseadragon" + id + @"',
            prefixUrl: '/openseadragon/images/',
            tileSources: {
                Image: {
                    xmlns: 'http://schemas.microsoft.com/deepzoom/2008',
                    Url: '"+url + id + @"/',
                    Format: 'jpg',
                    Overlap: '2',
                    TileSize: '256',
                    Size: {
                        Width: '" + cell.Root.Field(0).Get() + @"',
                        Height: '" + cell.Root.Field(1).Get() + @"'
                    }
                }
            }
        });  
    </script>";
            }

        }
    }

}
