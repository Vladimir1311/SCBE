using System;

namespace SituationCenterBackServer.Interfaces
{
    public interface IAccessValidator
    {
        /// <summary>
        /// Проверяет возможность человека получить доступ к ресурсу.
        /// </summary>
        /// <param name="userToken">Токен человека, который запрашивает доступ</param>
        /// <param name="targetFolder">Путь к желаемой папке, включает в себя id персоны владелья</param>
        /// <returns></returns>
        bool CanAccessToFolder(string userToken, string targetFolder);
    }
}
