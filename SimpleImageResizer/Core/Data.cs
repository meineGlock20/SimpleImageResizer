using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Core;

/// <summary>
/// Provides basic CRUD operations on a location sqlite database.
/// </summary>
/// <remarks>
/// Normally, if you were not using Entity Framework, the data access should be sererated into a data class library but
/// since this is very simple data access it was added here under one data class.
/// </remarks>
public sealed class Data
{
    private static readonly string connection = "data source = SIR.db";

    public static async Task<long> InsertProcessLog(Models.Process process)
    {
        string sql = $@"INSERT INTO ProcessLog
                        (
                            ProcessStart,
                            ProcessEnd,
                            ImageCount,
                            ImagesOriginalSize,
                            ImagesProcessedSize
                        )
                        VALUES
                        (
                            $ProcessStart,
                            $ProcessEnd,
                            $ImageCount,
                            $ImagesOriginalSize,
                            $ImagesProcessedSize
                        );";

        using SqliteConnection sqliteConnection = new(connection);
        using SqliteCommand sqliteCommand = new(sql, sqliteConnection);
        sqliteCommand.Parameters.Add("$ProcessStart", SqliteType.Text).Value = process.ProcessStart.ToString("yyyy-MM-dd HH:mm:ss.ff");
        sqliteCommand.Parameters.Add("$ProcessEnd", SqliteType.Text).Value = process.ProcessEnd.ToString("yyyy-MM-dd HH:mm:ss.ff");
        sqliteCommand.Parameters.Add("$ImageCount", SqliteType.Integer).Value = process.ImageCount;
        sqliteCommand.Parameters.Add("$ImagesOriginalSize", SqliteType.Integer).Value = process.ImagesOriginalSize;
        sqliteCommand.Parameters.Add("$ImagesProcessedSize", SqliteType.Integer).Value = process.ImagesProcessedSize;

        try
        {
            await sqliteConnection.OpenAsync();
            long rowsAffected = await sqliteCommand.ExecuteNonQueryAsync();
            sqliteConnection.Close();
            return rowsAffected;
        }
        catch(SqliteException)
        {
            if (sqliteConnection.State == System.Data.ConnectionState.Open)
                sqliteConnection.Close();
            throw;
        }
    }
}