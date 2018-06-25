namespace Storage
{
    public class StorageSetting
    {
        public string PathToUserSpaces { get; set; }
        public string PathToDocumentSpaces { get; set; }
        public string DocumentMetaFileName { get; set; }
        public string DocumentMetaPageExtension { get; set; }
        public string DocumentPaintExtension { get; set; }
    }

    public class PageManagerSetting
    {
        public int StartMaxTryCount { get; set; } = 40;
        public int StartTryDelay { get; set; } = 250;
        public int GetPageMaxTryCount { get; set; } = 10;
        public int GetPageTryDelay { get; set; } = 500;
    }
}