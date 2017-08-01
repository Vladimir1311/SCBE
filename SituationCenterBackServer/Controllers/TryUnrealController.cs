using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;

namespace SituationCenterBackServer.Controllers
{
    public class TryUnrealController : Controller
    {
        private IConnector _connector;
        private IStableConnector _stableConnector;

        public TryUnrealController(IConnector connector, IStableConnector stableConnector)
        {
            _connector = connector;
            _stableConnector = stableConnector;
        }

        public IActionResult Index()
        {
            return Json(new
            {
                Connector = _connector.GetType().FullName,
                StableConnector = _stableConnector.GetType().FullName
            });
        }
    }
}