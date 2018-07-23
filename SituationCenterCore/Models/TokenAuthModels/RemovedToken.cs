using System;
namespace SituationCenterCore.Models.TokenAuthModels
{
    public class RemovedToken
    {
        public Guid Id { get; set; }
        public Guid RemovedTokenId { get; set; }
        public DateTime RemovedTime { get; set; }
    }
}
