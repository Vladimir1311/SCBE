using System.IO;

namespace Storage.Interfaces
{
    /// <summary>
    /// Интерфейс хранилища
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Создать каталог по указанному пути
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        bool CreateDirectory(string Token, string Owner, string Path);

        /// <summary>
        /// Создать или перезаписать файл
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <param name="ContentStream">Поток с содержимым файла</param>
        /// <returns>Успех выполнения операции</returns>
        bool CreateFile(string Token, string Owner, string Path, Stream ContentStream);

        /// <summary>
        /// Возврящает сведения о папке и ее содержимом
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        IDirectoryDesc GetDirectoryInfo(string Token, string Owner, string Path);

        /// <summary>
        /// Возврящает сведения о файле
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        IFileDesc GetFileInfo(string Token, string Owner, string Path);

        /// <summary>
        /// Возврящает поток для чтеня файла
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Поток для чтения файла</returns>
        Stream GetFileContent(string Token, string Owner, string Path);

        /// <summary>
        /// Возврящает поток для чтения страницы документа
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <param name="Page">Индекс страницы документа, нумерация с 1</param>
        /// <returns>Поток для чтения файла</returns>
        Stream GetDocumentPageContent(string Token, string Owner, string Path, int Page);

        /// <summary>
        /// Возврящает поток для чтения векторного изображения на странице документа
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <param name="Page">Индекс страницы документа, нумерация с 1</param>
        /// <returns>Поток для чтения файла</returns>
        Stream GetDocumentOverlayContent(string Token, string Owner, string Path, int Page);

        /// <summary>
        /// Записывает векторное изображение на странице документа
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <param name="Page">Индекс страницы документа, нумерация с 1</param>
        /// <param name="Content">Поток с векторным изображением, если null то существующее изображение удаляется</param>
        /// <returns>Поток для чтения файла</returns>
        void SetDocumentOverlayContent(string Token, string Owner, string Path, int Page, Stream Content);

        /// <summary>
        /// Удаляет папку
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        bool DeleteDirectory(string Token, string Owner, string Path);

        /// <summary>
        /// Удаляет файл
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        bool DeleteFile(string Token, string Owner, string Path);

        /// <summary>
        /// Проверяет существует ли файл
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Существует ли файл</returns>
        bool IsExistFile(string Token, string Owner, string Path);

        /// <summary>
        ///Проверяет существует ли папка
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="Path">Путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Существует ли папка</returns>
        bool IsExistDirectory(string Token, string Owner, string Path);

        /// <summary>
        /// Перемещвет файл или папку
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="OldPath">Старый путь к ресурсу, относительно каталога Owner</param>
        /// <param name="NewPath">Новый путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        bool Move(string Token, string Owner, string OldPath, string NewPath);

        /// <summary>
        /// Копирует файл или папку
        /// </summary>
        /// <param name="Token">Токен для доступа к ресурсу</param>
        /// <param name="Owner">Хозяин ресурса</param>
        /// <param name="OldPath">Старый путь к ресурсу, относительно каталога Owner</param>
        /// <param name="NewPath">Новый путь к ресурсу, относительно каталога Owner</param>
        /// <returns>Успех выполнения операции</returns>
        bool Copy(string Token, string Owner, string OldPath, string NewPath);
    }
}