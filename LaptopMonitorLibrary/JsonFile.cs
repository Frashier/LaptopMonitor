using System.IO;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace LaptopMonitorLibrary
{
    public class JsonFile<T>
    {
        public readonly string Path;
        public List<T> Data
        {
            get
            {
                    return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(Path)) ?? new List<T>();
            }
        }

        public JsonFile(string path)
        {
            // Create empty file
            Path = path;
            File.WriteAllText(Path, "[]");
        }

        /// <summary>
        /// Append json data to json file
        /// </summary>
        /// <param name="data"></param>
        public void Append(T data)
        {
            List<T> buffer = Data;
            buffer.Add(data);

            string json = JsonConvert.SerializeObject(buffer);
            File.WriteAllText(Path, json);
        }

        /// <summary>
        /// Clear file content
        /// </summary>
        public void Clear()
        {
            File.WriteAllText(Path, "[]");
        }
    }
}
