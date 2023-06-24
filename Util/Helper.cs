using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


class Helper
{
    public string ConvertDataTableToJson(DataTable dataTable)
    {
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        foreach (DataRow dataRow in dataTable.Rows)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            foreach (DataColumn column in dataTable.Columns)
            {
                row[column.ColumnName] = dataRow[column];
            }
            rows.Add(row);
        }

        string json = JsonSerializer.Serialize(rows);
        return json;
    }
}