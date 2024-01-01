namespace DotBotBase.SQLite.Database;

public static class SQLiteUtils
{
    public static string GetSQLType(Type type)
    {
        if (type == typeof(string)) return "TEXT";
        else if (type == typeof(int)) return "INTEGER";
        else if (type == typeof(float)) return "FLOAT";
        else if (type == typeof(double)) return "DOUBLE";
        else if (type == typeof(bool)) return "BOOLEAN";
        else if (type == typeof(short)) return "SMALLINT";
        else if (type == typeof(byte)) return "TINYINT";
        else if (type == typeof(long)) return "BIGINT";
        else
            throw new Exception("Invalid SQL type " + type.Name);
    }
}