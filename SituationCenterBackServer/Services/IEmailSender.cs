using System.Threading.Tasks;

namespace SituationCenterBackServer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}