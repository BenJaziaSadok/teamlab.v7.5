/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ASC.Api.Web.Help.DocumentGenerator;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = System.IO.Directory;

namespace ASC.Api.Web.Help
{
    public class Searcher
    {
        private readonly string _indexDirectory;
        private FSDirectory _directory;
        private IndexSearcher searcher;
        private long _isOperational = 0;
        private static readonly object SynchLock = new object();

        public bool IsOperational
        {
            get { return Interlocked.Read(ref _isOperational) == 1; }
            private set
            {
                Interlocked.Exchange(ref _isOperational, value ? 1 : 0);
            }
        }

        public Searcher(string indexDirectory)
        {
            _indexDirectory = indexDirectory;
            if (!Directory.Exists(indexDirectory)) Directory.CreateDirectory(indexDirectory);
            try
            {
                _directory = FSDirectory.GetDirectory(_indexDirectory, false);
                searcher = new IndexSearcher(_directory);
            }
            catch (Exception)
            {
                IsOperational = false;
            }
        }

        public void CreateIndexIfNeeded(List<MsDocEntryPoint> documentation, DateTime? lastModified)
        {
            lock (SynchLock)
            {
                if (IsIndexingNeeded(lastModified) || !IsOperational)
                {
                    try
                    {
                        //Drop all index
                        if (searcher != null)
                            searcher.Close();
                        if (_directory != null)
                            _directory.Close();
                        //Delete dir
                        Directory.Delete(_indexDirectory, true);
                        _directory = FSDirectory.GetDirectory(_indexDirectory, true); //Reopen directory
                        var indexModifier = new IndexModifier(_directory, new StandardAnalyzer(), true);

                        foreach (var entryPoint in documentation)
                        {
                            var pointDoc = new Document();
                            //Id keys
                            pointDoc.Add(new Field("point", entryPoint.Name, Field.Store.YES, Field.Index.NOT_ANALYZED,
                                                   Field.TermVector.YES));
                            if (!string.IsNullOrEmpty(entryPoint.Summary))
                                pointDoc.Add(new Field("summary", entryPoint.Summary, Field.Store.YES,
                                                       Field.Index.ANALYZED, Field.TermVector.NO));
                            if (!string.IsNullOrEmpty(entryPoint.Example))
                                pointDoc.Add(new Field("example", entryPoint.Example, Field.Store.YES,
                                                       Field.Index.ANALYZED, Field.TermVector.NO));

                            foreach (var pointMethod in entryPoint.Methods)
                            {
                                var doc = new Document();
                                //Id keys
                                doc.Add(new Field("point", entryPoint.Name, Field.Store.YES, Field.Index.NOT_ANALYZED,
                                                  Field.TermVector.YES));
                                doc.Add(new Field("path", pointMethod.ToString(), Field.Store.YES,
                                                  Field.Index.NOT_ANALYZED, Field.TermVector.YES));

                                doc.Add(new Field("url", pointMethod.Path, Field.Store.YES, Field.Index.NOT_ANALYZED,
                                                  Field.TermVector.NO));
                                if (!string.IsNullOrEmpty(pointMethod.Notes))
                                    doc.Add(new Field("notes", pointMethod.Notes, Field.Store.YES, Field.Index.ANALYZED,
                                                      Field.TermVector.NO));
                                if (!string.IsNullOrEmpty(pointMethod.Remarks))
                                    doc.Add(new Field("remarks", pointMethod.Remarks, Field.Store.YES,
                                                      Field.Index.ANALYZED, Field.TermVector.NO));
                                if (!string.IsNullOrEmpty(pointMethod.Example))
                                    doc.Add(new Field("examlpe", pointMethod.Example, Field.Store.YES,
                                                      Field.Index.ANALYZED, Field.TermVector.NO));
                                if (!string.IsNullOrEmpty(pointMethod.Returns))
                                    doc.Add(new Field("returns", pointMethod.Returns, Field.Store.YES,
                                                      Field.Index.ANALYZED, Field.TermVector.NO));
                                if (!string.IsNullOrEmpty(pointMethod.ShortName))
                                    doc.Add(new Field("short", pointMethod.ShortName, Field.Store.YES,
                                                      Field.Index.ANALYZED, Field.TermVector.NO));
                                if (!string.IsNullOrEmpty(pointMethod.Summary))
                                    doc.Add(new Field("summary", pointMethod.Summary, Field.Store.YES,
                                                      Field.Index.ANALYZED, Field.TermVector.NO));
                                foreach (var param in pointMethod.Params)
                                {
                                    if (!string.IsNullOrEmpty(param.Description))
                                        doc.Add(new Field("param-" + param.Name + "-description", param.Description,
                                                          Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                                    if (!string.IsNullOrEmpty(pointMethod.Remarks))
                                        doc.Add(new Field("param-" + param.Name + "-remarks", param.Remarks,
                                                          Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                                }
                                indexModifier.AddDocument(doc);
                            }
                        }
                        indexModifier.Optimize();
                        indexModifier.Close();
                        searcher = new IndexSearcher(_directory);
                        IsOperational = true;
                    }
                    catch (Exception)
                    {
                        IsOperational = false;
                    }

                }
            }
        }

        private bool IsIndexingNeeded(DateTime? lastModified)
        {
            var indexFiles = Directory.GetFiles(_indexDirectory).Select(x => new FileInfo(x)).ToList();
            var doIndex = indexFiles.Any(x => x.LastWriteTimeUtc < lastModified) | !indexFiles.Any();
            return doIndex | true;//TODO: remove 'true'
        }

        public Dictionary<MsDocEntryPoint, Dictionary<MsDocEntryPointMethod, string>> Search(string query, List<MsDocEntryPoint> points)
        {
            var parser = new QueryParser("summary", new StandardAnalyzer());
            parser.SetDefaultOperator(QueryParser.Operator.AND);
            Query q = null;
            try
            {
                q = parser.Parse(MakeQuery(query));
            }
            catch (Lucene.Net.QueryParsers.ParseException)
            {
                q = parser.Parse(QueryParser.Escape(query));
            }

            var result = new Dictionary<MsDocEntryPoint, ICollection<MsDocEntryPointMethod>>();
            var hits = searcher.Search(q, Sort.RELEVANCE);
            for (int i = 0; i < hits.Length(); i++)
            {
                var doc = hits.Doc(i);
                var point = doc.Get("point");
                var method = doc.Get("path");
                if (!string.IsNullOrEmpty(method))
                {
                    var entryPoint = points.Single(x => x.Name == point);
                    var pointMethod = entryPoint.Methods.Single(x => x.ToString() == method);
                    ICollection<MsDocEntryPointMethod> foundPoints;
                    if (result.TryGetValue(entryPoint, out foundPoints))
                    {
                        foundPoints.Add(pointMethod);
                    }
                    else
                    {
                        foundPoints = new List<MsDocEntryPointMethod> { pointMethod };
                        result.Add(entryPoint, foundPoints);
                    }
                }
            }
            return result.ToDictionary(x => x.Key, y => y.Value.ToDictionary(key => key, value => string.Empty));
        }

        private string MakeQuery(string query)
        {
            return string.Format("url:{0} OR summary:{0} OR notes:{0} OR returns:{0} OR short:{0}", "\"" + query + "\"~5");
        }
    }
}