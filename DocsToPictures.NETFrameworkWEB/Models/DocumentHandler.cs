using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DocsToPictures.ClIProtocol;
using DocsToPictures.Models;
using Newtonsoft.Json;

namespace DocsToPictures.NETFrameworkWEB.Models
{
    public class DocumentHandler
    {
        private static string RenderClientExe => ConfigurationManager.AppSettings["PathToReneringClient"];
        private static string RenderClientUploads => ConfigurationManager.AppSettings["PathToReneringClientUploads"];

        private readonly Logger<DocumentHandler> logger = new Logger<DocumentHandler>();
        private readonly Document doc;

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
                    Arguments = $"\"{Path.Combine(doc.Folder, doc.Name)}\" \"{RenderClientUploads}\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
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
                    var line = await reader.ReadLineAsync();
                    logger.Debug($"readed >>{line}<<");
                    var message = JsonConvert.DeserializeObject<Message>(line);
                    switch (message.MessageType)
                    {
                        case MessageType.MetaReady:
                            var metaMessage = JsonConvert.DeserializeObject<MetaReadyMessage>(line);
                            doc.SetPagesCount(metaMessage.PagesCount);
                            break;
                        case MessageType.PageReady:
                            var pageReady = JsonConvert.DeserializeObject<PageReadyMessage>(line);
                            doc[pageReady.PageNum] = pageReady.PagePath;
                            break;
                        case MessageType.IncorrectDoc:
                            break;
                        case MessageType.Error:
                            throw new Exception("Error in proccess!");
                            break;
                        case MessageType.InvalidArgs:
                            throw new Exception("Invalid Args for proccess");
                            break;
                        case MessageType.Info:
                            break;
                        case MessageType.IncorrectOutputPath:
                            throw new Exception("Incorrect out Path");
                            break;
                        case MessageType.Finish:
                            return;
                        default:
                            break;
                    }
                }
            }
            try
            {
                await ReadFunc(proccess.StandardOutput);
            }
            catch (Exception ex)
            {
                logger.Warning("while read", ex);
            }
            proccess.WaitForExit();
            logger.Info($"doc with id {doc.Id}, exit code: {proccess.ExitCode}");
        }
    }
}