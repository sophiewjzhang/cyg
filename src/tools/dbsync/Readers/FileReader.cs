using dbsync.Abstractions;
using dbsync.Configuration;
using DTO.Abstractions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dbsync.Readers
{
    public class FileReader : Abstractions.IDataReader
    {
        private FileSystemReaderConfiguration config;

        private string GetFullPath(string fileName) => Path.Combine(config.SourcePath, DateTime.Now.Date.ToString("MM-dd-yyyy"), fileName);
        private string RemoveUnderscores(string value) => value.Replace("_", "");

        public FileReader(FileSystemReaderConfiguration config)
        {
            this.config = config;
        }

        private string NormalizeTimeStroke(string time)
        {
            var hoursStroke = time.Split(':');
            int hours = 0;
            if (int.TryParse(hoursStroke[0], out hours))
            {
                return $"{hours % 24}:{hoursStroke[1]}:{hoursStroke[2]}";
            }
            return null;
        }

        private object[] ToTypedValues<T>(IDictionary<string, string> source) where T : IEntity, new()
        {
            var results = new List<object>(source.Count());
            var resultType = typeof(T);

            foreach (var item in source)
            {
                try
                {
                    var property = resultType.GetProperty(RemoveUnderscores(item.Key), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var valueStroke = item.Value;
                    if (property.PropertyType == typeof(TimeSpan))
                    {
                        valueStroke = NormalizeTimeStroke(valueStroke);
                        results.Add(TimeSpan.Parse(valueStroke));
                    }
                    else
                    {
                        results.Add(Convert.ChangeType(valueStroke, property.PropertyType));
                    }
                }
                catch (FormatException e)
                {
                    Log.Warning(e, "Unable to convert property value to target type");
                }
            }

            return results.ToArray();
        }

        private Type GetPropertyType<T>(string propertyName)
        {
            var resultType = typeof(T);
            var property = resultType.GetProperty(RemoveUnderscores(propertyName), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return property.PropertyType;
        }

        public async Task<DataTable> ReadAsync<T>(string fileName) where T : IEntity, new()
        {
            var results = new DataTable("source");
            using (var reader = new StreamReader(GetFullPath(fileName)))
            {
                var keys = (await reader.ReadLineAsync()).Split(',');
                results.Columns.AddRange(keys.Select(x => new DataColumn(x, GetPropertyType<T>(x))).ToArray());
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(',');
                    var dictionary = keys.Zip(values, (s, i) => new { s, i })
                          .ToDictionary(item => item.s, item => item.i);
                    results.Rows.Add(ToTypedValues<T>(dictionary));
                }
            }

            return results;
        }
    }
}
