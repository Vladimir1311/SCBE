using DocsToPictures.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using CCF.Shared.Exceptions;

namespace Storage
{
    public class DocumentPageManager : StorageComponent, IDocumentPageManager
    {
        private ConcurrentQueue<Tuple<string, string>> renderDocQueue = new ConcurrentQueue<Tuple<string, string>>();
        private IDocumentProcessor DocsProcessor;
        private bool isWait = false;

        protected readonly PageManagerSetting ManagerSetting;
        protected readonly IServiceProvider ServiceProvider;
        private readonly ILogger _logger;

        public DocumentPageManager(ILogger<DocumentPageManager> logger, IOptions<StorageSetting> storageSetting, IOptions<PageManagerSetting> managerSetting, IServiceProvider serviceProvider, IFileSystem fileSystem, IDocumentProcessor docsProcessor)
            : base(storageSetting, fileSystem) 
            =>
            (ManagerSetting, ServiceProvider, DocsProcessor, _logger) = (managerSetting.Value, serviceProvider, docsProcessor, logger);

        public void ProcessDocument(string Owner, string path)
        {
            var realMetaDir = GetRealMetaDir(Owner, path);
            var real = GetRealPath(Owner, path);

            if (!FileSystem.DirectoryExists(realMetaDir))
                FileSystem.CreateDirectory(realMetaDir);

            SaveMeta(Owner, path, new DocumentMeta());

            if (DocsProcessor != null)
            {
                RunRecive(Owner, path);
            }
            else
            {
                AddToQueue(Owner, path);
            }
        }

        private void AddToQueue(string Owner, string path)
        {
            if (!isWait)
            {
                isWait = true;
                Task.Run(async () => await WaitDocProcessor());
            }
            renderDocQueue.Enqueue(Tuple.Create(Owner, path));
        }
        
        private async Task WaitDocProcessor()
        {
            _logger.LogWarning("DocProcessor is dead");

            while(DocsProcessor == null)
            {
                await Task.Delay(1000);
                DocsProcessor = (IDocumentProcessor)ServiceProvider.GetService(typeof(IDocumentProcessor));
            }

            _logger.LogWarning("DocProcessor is alive again");

            while (!renderDocQueue.IsEmpty)
            {
                renderDocQueue.TryDequeue(out var tuple);
                RunRecive(tuple.Item1, tuple.Item2);
            }

            isWait = false;
        }

        private void RunRecive(string Owner, string path)
        {
            var real = GetRealPath(Owner, path);

            var content = FileSystem.ReadInFile(real);
            var DocInProcessor = DocsProcessor.AddToHandle(FileSystem.GetFileName(real), content);

            Task.Run(async () => await RecivePageAsync(Owner, path, DocInProcessor));
        }

        private async Task RecivePageAsync(string Owner, string path, IDocument DocInProcessor)
        {
            try
            {
                var mata = new DocumentMeta();

                var tryLimit = ManagerSetting.StartMaxTryCount;
                while (DocInProcessor.PagesCount == -1)
                {
                    await Task.Delay(ManagerSetting.StartTryDelay);

                    if (--tryLimit == 0)
                    {
                        throw new Exception($"Так блэт. DocumentProccessor не дает мне картинки!\n{path}");
                    }
                }

                mata.NotReadyPage = new HashSet<int>(Enumerable.Range(1, DocInProcessor.PagesCount));
                mata.HavePaintPage = new HashSet<int>();

                SaveMeta(Owner, path, mata);

                var loopLimit = mata.NotReadyPage.Count() * ManagerSetting.GetPageMaxTryCount;

                while (mata.NotReadyPage.Count != 0 && --loopLimit != 0)
                {
                    await Task.Delay(ManagerSetting.GetPageTryDelay);

                    foreach (var pageIndex in DocInProcessor.GetAvailablePages().Union(mata.NotReadyPage).ToList())
                    {
                        var picName = GetRealPagePath(Owner, path, pageIndex);

                        using (var fs_read = DocInProcessor.GetPicture(pageIndex))
                        {
                            FileSystem.WriteToFile(picName, fs_read);
                        }

                        mata.NotReadyPage.Remove(pageIndex);
                    }
                }

                SaveMeta(Owner, path, mata);
            }
            catch (ServiceUnavailableException)
            {
                AddToQueue(Owner, path);
            }
        }
    }
}