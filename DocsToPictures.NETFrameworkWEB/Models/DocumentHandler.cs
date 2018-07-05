using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DocsToPictures.ClIProtocol;
using DocsToPictures.Models;
using Newtonsoft.Json;

namespace DocsToPictures.NETFrameworkWEB.Models
{
    public class DocumentHandler : IDisposable
    {
        private static string RenderClientExe => ConfigurationManager.AppSettings["PathToReneringClient"];
        private static string RenderClientUploads => ConfigurationManager.AppSettings["PathToReneringClientUploads"];

        private readonly Logger<DocumentHandler> logger = new Logger<DocumentHandler>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Document doc;
        private bool isRun = true;

        public Guid Id => doc.Id;
        public int Progress => doc.Progress;
        public Task CurrentTask { get; set; }
        public event Action Done;

        public DocumentHandler(Document doc)
        {
            this.doc = doc;
        }

        internal async Task Run()
        {
            var proccess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = RenderClientExe,
                    Arguments = $"\"{doc.Folder}\" \"{doc.Name}\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                }
            };
            try
            {
                proccess.Start();
            }
            catch (Exception ex)
            {
                logger.Warning("Error while start proccess", ex);
            }
            async Task ReadFunc(StreamReader reader)
            {
                while (true)
                {
                    if (cancellationTokenSource.IsCancellationRequested && isRun)
                    {
                        isRun = false;
                        logger.Debug("token source cancellation requested");
                        proccess.StandardInput.WriteLine("q");
                        logger.Trace("writed 'q' to proccess");
                    }
                    var line = await reader.ReadLineAsync();
                    logger.Trace($"readed >>{line}<<");
                    Message message = null;
                    try
                    {
                        message = JsonConvert.DeserializeObject<Message>(line);
                    }
                    catch (Exception ex)
                    {
                        logger.Warning("parsing failed", ex);
                        continue;
                    }
                    switch (message.MessageType)
                    {
                        case MessageType.MetaReady:
                            var metaMessage = JsonConvert.DeserializeObject<MetaReadyMessage>(line);
                            logger.Debug($"MetaReady: {metaMessage.PagesCount}");
                            doc.SetPagesCount(metaMessage.PagesCount);
                            break;
                        case MessageType.PageReady:
                            var pageReady = JsonConvert.DeserializeObject<PageReadyMessage>(line);
                            logger.Debug($"PageReady: {pageReady.PageNum}");
                            doc[pageReady.PageNum] = pageReady.PagePath;
                            break;
                        case MessageType.IncorrectDoc:
                            logger.Warning($"Send incorect doc to handling");
                            break;
                        case MessageType.Error:
                            logger.Warning($"Proccess throws error");
                            break;
                        case MessageType.InvalidArgs:
                            logger.Warning($"Passed invalid argumens");
                            break;
                        case MessageType.Info:
                            var infoMessage = JsonConvert.DeserializeObject<InfoMessage>(line);
                            logger.Debug($"Proccess send info >>{infoMessage.Message}<<");
                            break;
                        case MessageType.IncorrectOutputPath:
                            logger.Warning($"Incorrect output Path");
                            break;
                        case MessageType.Finish:
                            logger.Debug($"send Finish keyword");
                            return;
                        default:
                            logger.Warning($"Incorrect message type from proccess");
                            break;
                    }
                }
            }
            try
            {
                await ReadFunc(proccess.StandardOutput);
                var p = proccess.HasExited;
                proccess.WaitForExit();
                logger.Info($"doc with id {doc.Id}, exit code: {proccess.ExitCode}");
                Done?.Invoke();
            }
            catch (Exception ex)
            {
                logger.Warning("while read", ex);
            }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            logger.Trace("DISPOSE Cancelled token");
        }
    }
}