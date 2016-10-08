using System;
using System.IO;
using RDFTurtleParser;
using SparqlQuery.SparqlClasses;
using SparqlQuery.SparqlClasses.Query.Result;
using System.Collections.Generic;
using RamTripleStore;

namespace WebCassetteManager.Models
{
    public class CassetteContent
    {
        private readonly CassetteModel model;

      
      //  public List<TriplesGenerator> triplesFiles=new List<TriplesGenerator>();
        public CassetteContent(CassetteModel model)
        {
            this.model = model;

            //Store = new Store(@"C:\Users\Admin\Source\Repos\next\WebCassetteManager\WebCassetteManager\" +model.Name+ Guid.NewGuid().ToString().Replace("-", ""));
            // Store.ClearAll();

            //foreach (var file in new DirectoryInfo(model.Path + "/RDF").EnumerateFiles("*.ttl"))
            //{
            //    //     Store.AddFromTurtle(100, file.FullName);
            //    triplesFiles.Add(new TriplesGenerator(file.FullName));
            //}
            //Store.BuildIndexes();
            // Store.Start();
        }

        public IEnumerable<FileInfo> GetContent()
        {
            var rdfDirectory = new DirectoryInfo(model.Path + "/RDF");
            if(!rdfDirectory.Exists) yield break;
            foreach (var file in rdfDirectory.EnumerateFiles("*.ttl"))
            {
                //     Store.AddFromTurtle(100, file.FullName);
           // new TriplesGenerator(file.FullName).Start(eachtriple);
                yield return file;
            }
            
        }
    }

}