using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using RamTripleStore;
using WebCassetteManager.Models;

namespace WebCassetteManager
{
    public class CasssettesBD
    {
        public RdfTripleStore Store; //=new RdfTripleStore();
        public static readonly CasssettesBD CasssettesBd = new CasssettesBD();

        public CasssettesBD()
        {
            Cassettes = Load();
            Store = new RdfTripleStore();
            CassetteContents = new Dictionary<string, CassetteContent>();
            foreach (var cassetteModel in Cassettes)
            {
                LoadCassette(cassetteModel);
            }

        }

        public List<CassetteModel> Cassettes; // = Load(); //new List<CassetteModel>()
        public Dictionary<string, CassetteContent> CassetteContents; //= new Dictionary<string, CassetteContent>();



        public void SaveCassettes()
        {
            File.WriteAllText(
                @"C:\Users\Admin\Source\Repos\next\next\WebCassetteManager\WebCassetteManager\cassettes.json",
                new JavaScriptSerializer().Serialize(Cassettes));
        }

        public static List<CassetteModel> Load()
        {
            return
                new JavaScriptSerializer().Deserialize<List<CassetteModel>>(
                    File.ReadAllText(
                        @"C:\Users\Admin\Source\Repos\next\WebCassetteManager\WebCassetteManager\cassettes.json"));
        }

        public CassetteContent LoadCassette(CassetteModel model)
        {
            CassetteContent content;
            if (!CassetteContents.TryGetValue(model.Path, out content))
            {
                CassetteContents.Add(model.Path, content = new CassetteContent(model));
                foreach (var fileInfo in content.GetContent())
                {
                    Store.AddFromTurtle(0, fileInfo.FullName);
                    //  var @object = triple.Predicate == "uri"
                    //? triple.Object.ToString().Replace("iiss://", "iiss_")
                    //: triple.Object.ToString();
                    model.Status = "loaded";
                }
            }
            return content;
        }
    }
}