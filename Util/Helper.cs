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

    public dynamic getJsonProperty(string propertyName, string type, dynamic jsonElement)
    {
        dynamic value = null;
        if (jsonElement.TryGetProperty(propertyName, out JsonElement propertyElement) && type == "String"){
            value = propertyElement.GetString();
        }else if(jsonElement.TryGetProperty(propertyName, out JsonElement propertyElement2) && type == "Int"){
            if (propertyElement2.ValueKind == JsonValueKind.Number)
            {
                value = propertyElement2.GetInt32();
            }
            else if (propertyElement2.ValueKind == JsonValueKind.String && int.TryParse(propertyElement2.GetString(), out int parsedNumber))
            {
                value = parsedNumber;
            }
        }else if(jsonElement.TryGetProperty(propertyName, out JsonElement propertyElement3) && type == "Double"){
            if (propertyElement2.ValueKind == JsonValueKind.Number)
            {
                value = propertyElement2.GetDouble();
            }
            else if (propertyElement2.ValueKind == JsonValueKind.String && double.TryParse(propertyElement2.GetString(), out double parsedNumber))
            {
                value = parsedNumber;
            }
        }
        return value;
    }
}