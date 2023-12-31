namespace DotBotBase.Core.Database;

public interface IDatabase
{
    public string Name { get; }

    public IDbTable[] GetTables();
    public T GetOrCreateTable<T>(string name, IDbTable table) where T : IDbTable;
    public T? GetTable<T>(string name) where T : IDbTable;
    public void CreateTable<T>(T table) where T : IDbTable;
    public void UpdateTable<T>(T table) where T : IDbTable;
    public void DeleteTable<T>(T table) where T : IDbTable;
}