using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsToPictures.Models;
using DocsToPictures.Interfaces;
using System.Threading;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace DocsToPictures
{
    class Program
    {
        static void Main(string[] args)
        {
            LogsWriter.Info($"args is {string.Join("---", args)}");
            if (args.Length != 2)
            {
                LogsWriter.Info($"not two args");
                LogsWriter.InvalidArgs();
                return;
            }
            var directory = args[0];
            var fileName = args[1];
            var filePath = Path.Combine(directory, fileName);


            if (!File.Exists(filePath))
            {
                LogsWriter.Info($"file no exist");
                LogsWriter.IncorrectDoc();
                return;
            }
            if (!Directory.Exists(directory))
            {
                LogsWriter.Info($"no dir");
                LogsWriter.IncorrectOutputPath();
                return;
            }
            try
            {
                GeneralWork(fileName, directory);
            }
            catch (Exception ex)
            {
                LogsWriter.Info($"{ex.Message}\n {ex.StackTrace}");
                if (ex.InnerException != null)
                    LogsWriter.Info($"Inner exception: {ex.InnerException.Message}\n {ex.InnerException.StackTrace}");
            }
        }

        static void GeneralWork(string fileName, string directory)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cancellationSource = new CancellationTokenSource();

            var handler = Assembly.GetAssembly(typeof(DocumentHandler))
                .GetTypes()
                .Where(T => T.IsSubclassOf(typeof(DocumentHandler)))
                .Where(T => T.GetCustomAttributes<SupportedFormatAttribyte>().Any(attr => attr.Format == Path.GetExtension(fileName)))
                .Select(T => Activator.CreateInstance(T, cancellationSource.Token) as DocumentHandler)
                .DefaultIfEmpty(null)
                .Single();

            if (handler == null)
            {
                LogsWriter.IncorrectDoc();
                cancellationSource.Cancel();
                return;
            }

            var document = new Document
            {
                Folder = directory,
                Name = fileName,
                Id = Guid.NewGuid()
            };

            document.MetaReadyEvent += (pCount) =>
            {
                LogsWriter.MetaReady(pCount);
            };
            document.PageReady += (pNum, pPath) =>
            {
                LogsWriter.PageReady(pNum, pPath);
            };

            handler.Initialize();
            handler.AddToHandle(document);

            while (document.GetAvailablePages().Count != document.PagesCount)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadLine() == "q")
                    {
                        cancellationSource.Cancel();
                        break;
                    }
                }
                LogsWriter.Info($"Waiting {stopwatch.ElapsedMilliseconds} milliseconds");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            LogsWriter.Info($"Finish, time: {stopwatch.ElapsedMilliseconds} milliseconds");
            LogsWriter.Finish();
            cancellationSource.Cancel();
            handler.Dispose();
        }
    }
}
