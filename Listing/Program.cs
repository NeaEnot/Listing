using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Listing
{
    class Program
    {
        public static IConfiguration configuration;

        static void Main(string[] args)
        {
            configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

            List<FileInfo> files = getAllFiles(configuration.GetSection("path").Value);
            List<FileInfo> operatingFiles = new List<FileInfo>();
            foreach (var extension in configuration.GetSection("extensions").GetChildren())
            {
                operatingFiles.AddRange(files.Where(file => file.Extension == extension.Value).ToList());
            }

            using (StreamWriter writer = new StreamWriter(configuration.GetSection("outPath").Value)) ;

            foreach (var file in operatingFiles)
            {
                Console.WriteLine(file.FullName.Substring(configuration.GetSection("path").Value.Length));

                string text = "";

                using (StreamReader reader = new StreamReader(file.FullName))
                {
                    text = reader.ReadToEnd();
                }

                using (StreamWriter writer = new StreamWriter(configuration.GetSection("outPath").Value, true))
                {
                    writer.WriteLine("/*" + file.FullName.Substring(configuration.GetSection("path").Value.Length) + "*/");
                    writer.WriteLine(text);
                    writer.WriteLine("//|||||||||||||||||||||||||||||||||||||||||||||||||||");
                }
            }
        }

        static List<FileInfo> getAllFiles(string path)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(path);

            files.AddRange(directory.GetFiles());

            var directories = directory.GetDirectories();
            foreach (var dir in directories)
            {
                bool isIgnore = false;

                foreach (var ign in configuration.GetSection("ignoreDirs").GetChildren())
                {
                    if (dir.Name == ign.Value)
                    {
                        isIgnore = true;
                        break;
                    }
                }

                if (isIgnore)
                {
                    continue;
                }

                files.AddRange(getAllFiles(dir.FullName));
            }

            return files;
        }
    }
}