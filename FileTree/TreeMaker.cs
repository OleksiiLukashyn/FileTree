using System;
using System.IO;
using System.IO.Compression;

namespace FileTree
{
    internal class TreeMaker
    {
        private const string Source = "Tree.txt";
        private const string Compressed = "Tree.zip";

        public static void MakeZippedTree()
        {
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var searchPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            using (var outputFile = new StreamWriter(Path.Combine(docPath, Source)))
            {
                DirectorySearch(searchPath, outputFile);
            }
            Compress(Path.Combine(docPath, Source), Path.Combine(docPath, Compressed));
        }

        private static void DirectorySearch(string sDir, TextWriter textWriter, int level = 0)
        {
            try
            {
                var directoryPrefix = "";
                for (var i = 0; i < level; i++)
                    directoryPrefix += "|———";
                var filePrefix = directoryPrefix + "|———";
                level++;
                foreach (var directory in Directory.GetDirectories(sDir))
                {
                    textWriter.WriteLine(string.Concat(directoryPrefix, Path.GetFileName(directory)));
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        if (File.GetCreationTime(file) < DateTime.Now.AddDays(-14))
                            textWriter.WriteLine(string.Concat(filePrefix, Path.GetFileName(file)));
                    }
                    DirectorySearch(directory, textWriter, level);
                }
            }
            catch (Exception exception)
            {
                textWriter.WriteLine(exception.Message);
            }
        }

        private static void Compress(string source, string compressed)
        {
            using (var fs = new FileStream(source, FileMode.OpenOrCreate))
            {
                using (var cfs = File.Create(compressed))
                {
                    using (var gzs = new GZipStream(cfs, CompressionMode.Compress))
                    {
                        fs.CopyTo(gzs);
                    }
                }
            }
            File.Delete(source);
        }
    }
}
