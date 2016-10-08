using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace RepoInfo
{
    class Repository2Rdf
    {
        private  string cassetteName="new_casssette";
        private Repository repository;
        private static readonly Signature SignatureCreator = new Signature("creator", "creator@iis.nsk.su", DateTimeOffset.UtcNow);

        public Repository2Rdf(string path = @"C:\cassettes\new_casssette")
        {
            cassetteName = Path.GetDirectoryName(path);
            if (!Repository.IsValid(path))
            {
                string gitDirPath = Repository.Init(path);
                repository = new Repository(path);
                foreach (var dir in new DirectoryInfo(path + "/originals").EnumerateDirectories())
                    foreach (var file in dir.EnumerateFiles())
                    {
                        repository.Index.Add("originals/" + dir.Name + "/" + file.Name);
                    }
                repository.Commit("first1", SignatureCreator, SignatureCreator);
            }
            else
                repository = new Repository(path);
        }

        public void Add(IEnumerable<string> relativePaths, Signature user)
        {
            foreach (var relativePath in relativePaths)
            {
           repository.Index.Add(relativePath);
            }
            repository.Commit("add", user, user);
        }

        public void Remove(IEnumerable<string> relativePaths, Signature user)
        {
            foreach (var relativePath in relativePaths)
            {
                repository.Index.Remove(relativePath);
            }
            repository.Commit("remove", user, user);
        }

        public IEnumerable<Tuple<string, string, string>> GetRDF()
        {

            //foreach (var dir in new DirectoryInfo(path + "/originals").EnumerateDirectories())
            //    foreach (var file in dir.EnumerateFiles())
            //    {
            //        repository.Index.Add("originals/" + dir.Name + "/" + file.Name);
            //    }
            //commit = repository.Commit("first1", signature, signature);
            //   repository = new Repository(path);
            //   repository.Index.Add("originals/0001/13fd39c5f6af5c8d7aed71dc00dd6d21.jpg");
            //   commit = repository.Commit("3", signature, new Signature("autor", "@", DateTimeOffset.UtcNow));
            var commit = repository.Commits.First();
            yield return Tuple.Create("cassette root collection",
                "contains", commit.Tree.Sha);

            foreach (var tuple in FromTree(commit.Tree))
                yield return tuple;
            //var repositoryStatus = repository.RetrieveStatus(new StatusOptions {RecurseIgnoredDirs=true,DetectRenamesInIndex = false, Show = StatusShowOption.WorkDirOnly});
            //var ignoredhashSet = new HashSet<string>(repositoryStatus.Ignored.Select(entry => entry.FilePath));
            //foreach (var file in new DirectoryInfo(path).EnumerateFiles("*.*", SearchOption.AllDirectories)
            //    .Where(file => ignoredhashSet.Contains(file.FullName)))
            //{
            //    repository.Commits.QueryBy(file.FullName);
            //}
        }

        private  IEnumerable<Tuple<string, string, string>> FromTree(Tree tree)
        {
            foreach (var treeNode in tree)
            {
                yield return Tuple.Create(tree.Sha, "contains", treeNode.Target.Sha);
                yield return Tuple.Create(treeNode.Target.Sha, "path", treeNode.Path);
                if (treeNode.TargetType == TreeEntryTargetType.Tree)
                {
                    foreach (var tuple in FromTree((Tree) treeNode.Target))
                    {
                        yield return tuple;
                    }
                }
                else
                {
                    
                    var lastchanges = repository.Commits.QueryBy(treeNode.Path).First().Commit;
                    //var createdcomit = repository.Commits.QueryBy(treeNode.Path).Last().Commit;
                    //Console.WriteLine(repository.Commits.QueryBy(treeNode.Path).Last().Commit.Message);
                    //yield return Tuple.Create(treeNode.Target.Sha, "added time", createdcomit.Author.When.ToString());
                    //yield return Tuple.Create(treeNode.Target.Sha, "added autor name", createdcomit.Author.Name);
                    //yield return Tuple.Create(treeNode.Target.Sha, "added autor email", createdcomit.Author.Email);

                    yield return Tuple.Create(treeNode.Target.Sha, "last changes time", lastchanges.Author.When.ToString());
                    yield return Tuple.Create(treeNode.Target.Sha, "last canges author name", lastchanges.Author.Name);
                    yield return Tuple.Create(treeNode.Target.Sha, "last canges author email", lastchanges.Author.Email);
                    yield return Tuple.Create(treeNode.Target.Sha, "uri", cassetteName+"@iis.nsk.su/0001/"+Path.GetFileNameWithoutExtension(treeNode.Path.Substring(10)));
                    yield return Tuple.Create(treeNode.Target.Sha, "ext", Path.GetExtension(treeNode.Path));
                }
            }
        }
    }
}
