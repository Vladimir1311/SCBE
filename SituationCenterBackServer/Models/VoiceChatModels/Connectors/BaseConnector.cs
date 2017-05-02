using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels.Connectors
{
    public abstract class BaseConnector
    {
        protected Func<string, ApplicationUser> _findUserFunc;
        
        private readonly Dictionary<ApplicationUser, IConnector> _connectionForUsers = new Dictionary<ApplicationUser, IConnector>();

        public void SetBindToUser(Func<string, ApplicationUser> findUserFunc)
        {
            _findUserFunc = findUserFunc;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
