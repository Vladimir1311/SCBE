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

namespace DocsToPictures
{
    class Program
    {
        static void Main(string[] args)
        {
            LogsWriter.Info($"args is {string.Join("---", args)}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (args.Length != 2)
            {
                LogsWriter.Info($"not two args");
                LogsWriter.InvalidArgs();
                return;
            }
            if (!File.Exists(args[0]))
            {
                LogsWriter.Info($"file no exist");
                LogsWriter.IncorrectDoc();
                return;
            }
            if (!Directory.Exists(args[1]))
            {
                LogsWriter.Info($"no dir");
                LogsWriter.IncorrectOutputPath();
                return;
            }
            IDocumentProcessor processor = new DocumentProcessor(args[1]);
            LogsWriter.Info($"extensions: {string.Join("---", processor.GetSupportedExtensions())}");
            if (!processor.GetSupportedExtensions().Contains(Path.GetExtension(args[0])))
            {
                LogsWriter.IncorrectDoc();
                processor.Dispose();
                return;
            }
            var document = processor.AddToHandle(Path.GetFileName(args[0]), File.OpenRead(args[0]));
            document.MetaReadyEvent += (pCount) =>
            {
                LogsWriter.MetaReady(pCount);
            };
            document.PageReady += (pNum, pPath) =>
            {
                LogsWriter.PageReady(pNum, pPath);
            };
            while (document.GetAvailablePages().Count != document.PagesCount)
            {
                LogsWriter.Info($"Waiting {stopwatch.ElapsedMilliseconds} milliseconds");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            LogsWriter.Info($"Finish, time: {stopwatch.ElapsedMilliseconds} milliseconds");
            LogsWriter.Finish();
            processor.Dispose();
        }
    }
}
