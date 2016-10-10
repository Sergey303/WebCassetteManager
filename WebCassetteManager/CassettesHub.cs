using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses;
using WebCassetteManager.Models;

namespace WebCassetteManager
{
    public class CassettesHub : Hub
    {
        public void GetDirectValue(string subj, string predicate)
        {
            foreach (var objectVariant in CasssettesBD.CasssettesBd.Store.GetTriplesWithSubjectPredicate(new OV_iri(subj), new OV_iri(predicate)))
            {
                Clients.Caller.addtriple(subj, predicate, objectVariant.ToString(), objectVariant.Variant==ObjectVariantEnum.Iri);
            }
        }
        public void GetInverseValue(string obj, string predicate)
        {
            
            foreach (var subjVariant in CasssettesBD.CasssettesBd.Store.GetTriplesWithPredicateObject(new OV_iri(predicate), new OV_iri(obj)))
            {
                Clients.Caller.addTriple(subjVariant.ToString(), predicate, obj);
            }
            
        }
        public void GetAllDirect(params string[]subjPreds)
        {
            for (int i = 0; i < subjPreds.Length-1; i+=2)
            {
                string subj=subjPreds[i];
                string predicate = subjPreds[i + 1];
                foreach (var objectVariant in CasssettesBD.CasssettesBd.Store.GetTriplesWithSubjectPredicate(new OV_iri(subj), new OV_iri(predicate)))
                {
                    Clients.Caller.addtriple(subj, predicate, objectVariant.ToString(), objectVariant.Variant == ObjectVariantEnum.Iri);
                }
            }
        }
        List<string[]> buffer = new List<string[]>();
        public void Return2Client(string subject, string predicate, string @object, bool isobj)
        {
                    Clients.Caller.addtriple(subject, predicate, @object, isobj);
        }



        ///"^in-collection/collection-item/(name,uri)"
        public void GetTriplesFromPath(string subject, string path)
        {
            GetTriplesFromPathRecursive(subject, path);
          //  SendBuffered();
        }

        private void GetTriplesFromPathRecursive(string subject, string path)
        {
            int indexOfNext = path.IndexOf('/');

            if (path.StartsWith("("))
            {
                var predicates = path.Split(new[] {"(", ")", ","}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pred in predicates)
                {
                    if (pred[0] == '^')
                    {
                        var predicate = pred.Substring(1);
                        foreach (var subjectVariant in
                            CasssettesBD.CasssettesBd.Store.GetTriplesWithPredicateObject(new OV_iri(predicate),
                                new OV_iri(subject)))
                            //   Clients.Caller.addtriple(subjectVariant.ToString(), predicate, subject, true);
                            Return2Client(subjectVariant.ToString(), predicate, subject, true);
                    }
                    else
                    {
                        var predicate = pred;
                        foreach (var objectVariant in
                            CasssettesBD.CasssettesBd.Store.GetTriplesWithSubjectPredicate(new OV_iri(subject),new OV_iri(predicate)))
                            Return2Client(subject, predicate, objectVariant.ToString(),
                                objectVariant.Variant == ObjectVariantEnum.Iri);
                        //Clients.Caller.addtriple(subject, predicate, objectVariant.ToString(), true);
                    }
                }
            }
            else
            {
                if (path[0] == '^')
                {
                    var predicate = indexOfNext == -1 ? path.Substring(1) : path.Substring(1, indexOfNext-1);
                    foreach (var subjectVariant in
                        CasssettesBD.CasssettesBd.Store.GetTriplesWithPredicateObject(new OV_iri(predicate), new OV_iri(subject))
                        )
                    {
                        //Clients.Caller.addtriple(subjectVariant.ToString(), predicate, subject, true);
                        Return2Client(subjectVariant.ToString(), predicate, subject, true);

                        if (indexOfNext == -1) continue;
                        GetTriplesFromPathRecursive(subjectVariant.ToString(), path.Substring(indexOfNext + 1));
                    }
                }
                else
                {
                    var predicate = indexOfNext == -1 ? path : path.Substring(0, indexOfNext-1);
                    foreach (var objectVariant in
                        CasssettesBD.CasssettesBd.Store.GetTriplesWithSubjectPredicate(new OV_iri(subject),
                            new OV_iri(predicate)))
                    {
                        Return2Client(subject, predicate, objectVariant.ToString(),
                            objectVariant.Variant == ObjectVariantEnum.Iri);
                        //Clients.Caller.addtriple(subject, predicate, objectVariant.ToString(), true);
                        if (indexOfNext == -1) continue;
                        GetTriplesFromPathRecursive(objectVariant.ToString(), path.Substring(indexOfNext+1));
                    }
                }
            }
        }


        //public void GetCassetes()
        //{
        //    Clients.Caller.get(CasssettesBD.CasssettesBd.Cassettes); //.addNewMessageToPage(name, message);
        //}

        //public void CallSparql( string sparql)
        //{
        //    Clients.Caller.getCasssetteContent(SparqlQueryParser.Parse(CasssettesBD.CasssettesBd.Store, sparql).Run().ToJson());
        //}

        //private CassetteContent LoadCassette(CassetteModel model)
        //{

        //    Clients.All.updateCassetteStatus(model);
        //    var content = CasssettesBD.CasssettesBd.LoadCassette(model);
        //    Clients.All.updateCassetteStatus(model);

        //    return content;
        //}
        //public void GetItem( string id)
        //{
        //    CasssettesBD.CasssettesBd.Store.Describe(id, (subj, pred, obj, isIri) =>
        //    {
        //        //var @object = pred == "uri"
        //        //    ? obj.ToString().Replace("iiss://", "iiss_")
        //        //    : obj.ToString();
        //        Clients.Caller.updateArrayProperty(subj, pred, obj.ToString(), isIri);
        //    });
        //   // Clients.Caller.alltripletsDone(id);


        //    //ObjectVariants codedId;
        //    //if (!content.Store.NodeGenerator.TryGetUri(new OV_iri(id), out codedId)) return;

        //    //foreach (var triple in content.Store
        //    //    .GetTriplesWithSubject(codedId))
        //    //    Clients.Caller.updateArrayProperty(id, triple.Predicate.ToString(), triple.Object.ToString());
        //    ////foreach (var uri in content.Store
        //    ////    .GetTriplesWithSubjectPredicate(codedId, content.Store.NodeGenerator.AddIri("uri"))
        //    ////    .Cast<OV_string>()
        //    ////    .Select(ovstr => ovstr.value))
        //    ////    Clients.Caller.updateArrayProperty(id, "uri", uri.Replace("iiss://", "iiss_"));

        //    ////foreach (var type in content.Store
        //    ////    .GetTriplesWithSubjectPredicate(codedId, content.Store.NodeGenerator.AddIri("documenttype"))
        //    ////    .Cast<OV_string>()
        //    ////    .Select(ovstr => ovstr.value))
        //    ////    Clients.Caller.updateArrayProperty(id, "documenttype", type);

        //    //foreach (var parent in 
        //    //    content.Call(
        //    //        $@"CONSTRUCT {{<{id}> <parent> ?parentId .
        //    //                        ?parentId   <name> ?parentName.
        //    //                        ?parentId   <sub> <{id}>.
        //    //                                                }}
        //    //           WHERE{{<{id}> ^<collection-item> / <in-collection> ?parentId .
        //    //                                               OPTIONAL {{  ?parentId   <name> ?parentName.  }}")
        //    //        .Results.Select(SparqlResultSet.ToJson)
        //    //        .Select(s => new JavaScriptSerializer().Deserialize<ExpandoObject>(s))
        //    //        .Cast<dynamic>()
        //    //    )
        //    //{
        //    //    Clients.Caller.updateArrayProperty(id, "parent", parent);
        //    //    Clients.Caller.updateArrayProperty(parent.parentId.value, "sub", id);
        //    //}
        //    //foreach (var sub in content.Call($@"select ?sub {{?sub ^<collection-item> / <in-collection> <{id}> }}
        //    //                                                    OPTIONAL {{  ?sub   <name> ?subName.  }}
        //    //                                                    OPTIONAL {{  ?sub   <documenttype> ?subDoctype.  
        //    //                                                                 ?sub   <uri> ?subDoctype.  }}")
        //    //            .Results.Select(SparqlResultSet.ToJson))
        //    //    Clients.Caller.updateProperty(id, "sub", sub, true);
        //}


        //public void SendProperty(object sender, PropertyChangedEventArgs e)
        //{
        //    var type = sender.GetType();
        //    base.Clients.Caller.updateProperty(
        //     type.GetProperty("Id").GetValue(sender, null),
        //     e.PropertyName,
        //     type.GetProperty(e.PropertyName).GetValue(sender, null));
        //}

        //class CollectionModel :INotifyCollectionChanged, INotifyPropertyChanged
        //{

        //    public event NotifyCollectionChangedEventHandler CollectionChanged;
        //    public event PropertyChangedEventHandler PropertyChanged;

        //    [NotifyPropertyChangedInvocator]
        //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

         
    }
}