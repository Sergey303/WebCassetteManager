using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon;
using RDFCommon.OVns;
using RDFTurtleParser;

namespace RamTripleStore
{
    public struct PredicateTarget
    {
        public string Predicate;
        public ObjectVariants Target;
    }
    public class RdfTripleStore :IStore
    {
        Dictionary<ObjectVariants, List<PredicateTarget>[]> dictionary=new Dictionary<ObjectVariants, List<PredicateTarget>[]>();
        public RdfTripleStore()
        {
            
        }

        public void Load(IEnumerable<TripleStrOV> flow)
        {
            foreach (var tripleStrOv in flow)
                Add(tripleStrOv.Subject, tripleStrOv.Predicate, tripleStrOv.Object);
        }

        private void Add(string subject, string predicate, ObjectVariants @object)
        {
            List<PredicateTarget>[] item;
            ObjectVariants subjectOV=new OV_iri(subject);
            if (!dictionary.TryGetValue(subjectOV, out item))
                dictionary.Add(subjectOV, item = new List<PredicateTarget>[2]);
            if (item[0] == null) item[0] = new List<PredicateTarget>();

            item[0].Add(new PredicateTarget()
            {
                Predicate = predicate,
                Target = @object,
            });
            if (@object.Variant != ObjectVariantEnum.Iri) return;
            if (!dictionary.TryGetValue(@object, out item))
                dictionary.Add(@object, item = new List<PredicateTarget>[2]);

            if (item[1] == null) item[1] = new List<PredicateTarget>();
            item[1].Add(new PredicateTarget()
            {
                Predicate = predicate,
                Target = new OV_iri(subject)
            });
        }

        public void Describe(string iri, Action<string, string, ObjectVariants, bool> tripleAction)
        {
            List<PredicateTarget>[] item;
            ObjectVariants iriOv = new OV_iri(iri);
            if (!dictionary.TryGetValue(iriOv, out item)) return;
            {
                foreach (var predicateTarget in item[0])
                    tripleAction(iri, predicateTarget.Predicate, predicateTarget.Target, predicateTarget.Target.Variant==ObjectVariantEnum.Iri);
                foreach (var predicateTarget in item[1])
                    tripleAction(predicateTarget.Target.ToString(), predicateTarget.Predicate, iriOv, true);
            }
        }

        #region IStore

        public string Name { get; set; }
        public NodeGenerator nodeGenerator = new NodeGenerator();

        public NodeGenerator NodeGenerator
        {
            get { return nodeGenerator; }
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            Add(s.ToString(), p.ToString(), o);
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void Build(IEnumerable<TripleStrOV> triples)
        {
            throw new NotImplementedException();
        }

        public void Build(long nodesCount, IEnumerable<TripleStrOV> triples)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public void AddFromTurtle(long iri_Count, string gString)
        {
            new TripleGeneratorBuffered(gString, "default").Start(ovs =>
            {
                    foreach (var tripleStrOv in ovs)
                    {
                        Add(tripleStrOv.Subject, tripleStrOv.Predicate, tripleStrOv.Object);
                    }
            });
        }

        public void FromTurtle(string fullName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetSubjects(ObjectVariants pred, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public long GetTriplesCount()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithObject(ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithTextObject(ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithPredicate(ObjectVariants p)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithSubject(ObjectVariants s)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            List<PredicateTarget>[] list;
            return dictionary.TryGetValue(subj, out list)
                ? list[0].Where(targets => targets.Predicate == pred.ToString()).Select(target => target.Target)
                : Enumerable.Empty<ObjectVariants>();
        }
        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            List<PredicateTarget>[] list;
            return dictionary.TryGetValue(obj, out list)
                ? list[1].Where(targets => targets.Predicate == pred.ToString()).Select(target => target.Target)
                : Enumerable.Empty<ObjectVariants>();
        }
        public void Warmup()
        {
            throw new NotImplementedException();
        }

        public IStoreNamedGraphs NamedGraphs { get; }

        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        public IGraph CreateTempGraph()
        {
            throw new NotImplementedException();
        }

        public void ReloadFrom(string filePath)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
