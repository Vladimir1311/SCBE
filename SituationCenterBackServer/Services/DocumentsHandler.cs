﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SituationCenterBackServer.Models.DocumentHandlingModels;
using SituationCenterBackServer.Models.StorageModels;
using Microsoft.Extensions.Options;
using SituationCenterBackServer.Models.Options;
using System.Net.Http;
using IO = System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SituationCenterBackServer.Services
{
    public class DocumentsHandler : IDocumentHandlerService
    {
        public event Action<(string filePath, int pageNum)> OnPageDone;

        private Timer docsUpdater;
        private HttpClient httpClient;
        private readonly DocumentsHandlerConfiguration options;
        private ConcurrentDictionary<string, File> handlingFiles;
        private readonly IStorageManager storageManager;
        private readonly ILogger<DocumentsHandler> logger;

        public DocumentsHandler(IOptions<DocumentsHandlerConfiguration> options,
            IStorageManager storageManager,
            ILogger<DocumentsHandler> logger)
        {
            this.logger = logger;
            this.options = options.Value;
            this.storageManager = storageManager;
            handlingFiles = GetfilesIsHandling();
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.options.EndPoint)
            };
            docsUpdater = new Timer(UpdateHandledDocuments, null, 1000, 1000);
        }

        

        public void FillState(File file)
        {
            FillStates(new File[] { file });
        }

        public void FillStates(IEnumerable<File> files)
        {
            foreach (File file in files)
            {
                if (handlingFiles.TryGetValue(file.Path, out var handled))
                {
                    file.State = handled.State;
                    file.Progress = handled.Progress;
                    continue;
                }
                file.State = FileReadyState.Ready;
                file.Progress = 100;
            }
        }

        public bool IsSupported(string format)
        {
            return options.SupportedFormats.Contains(format);
        }

        public (bool success, string message) SendDocumentToHandle(File document)
        {
            if (!IsSupported(IO.Path.GetExtension(document.Name)))
                throw new NotSupportedException($"{document.Name} extension not supported");
            byte[] data;
            var docStream = storageManager.GetFileStream(document.Path);
            using (var br = new IO.BinaryReader(docStream))
                data = br.ReadBytes((int)docStream.Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent
                {
                    { bytes, "file", document.Name}
                };
            var result = httpClient.PostAsync("documenttopictures/load", multiContent)
                .Result
                .Content
                .ReadAsStringAsync()
                .Result;
            var response = JsonConvert.DeserializeObject<DocimentAddedResponse>(result);
            document.Id = response.Object.PackId;
            document.Progress = 0;
            document.State = FileReadyState.InQueue;
            handlingFiles[document.Path] = document;
            return (response.Success, response.Message);
        }
        public IO.Stream GetPicture(string filePath, int pageNum)
        {
            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(options.EndPoint) })
            {
                throw new NotImplementedException();
            }
        }

        private ConcurrentDictionary<string, File> GetfilesIsHandling()
        {
            return new ConcurrentDictionary<string, File>();
        }

        private void UpdateHandledDocuments(object mock)
        {
            try
            {
                var files = handlingFiles.Values.ToList();
                logger.LogInformation($"Getted {files.Count} files");
                foreach (var file in files)
                {
                    var result = httpClient.GetAsync("DocumentToPictures/getinfo?docid=" + file.Id)
                        .Result
                        .Content
                        .ReadAsStringAsync()
                        .Result;
                    logger.LogInformation(result);
                    var response = JsonConvert.DeserializeObject<GetDocumentInfoResponse>(result);
                    if (!response.Success)
                        continue;
                    if (response.Object.Progress > 0)
                        file.State = FileReadyState.Handling;
                    if (response.Object.Progress == 100)
                        file.State = FileReadyState.Ready;
                    file.Progress = response.Object.Progress;
                    foreach(var num in response.Object.AvailablePages)
                    {
                        if (file.Pictures.Count > num)
                            continue;
                        file.Pictures.AnyInsert(num, new Picture { State = PictureState.Handling });
                    }
                }
            }
            catch { }
        }

    }
}
