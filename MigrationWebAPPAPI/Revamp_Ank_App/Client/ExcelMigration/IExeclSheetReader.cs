namespace Revamp_Ank_App.Client.ExcelMigration
{
    public interface IExeclSheetReader
    {
        Task<bool> CreateCollectionUsingExcl(string filePath, string sheetName);
    }
}
