using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Runtime;

namespace LaptopMonitorLibrary
{
    public class JsonFile<T>
    {
        /// <summary>
        /// Path to .json file
        /// </summary>
        public readonly string FilePath;
        /// <summary>
        /// Whether data should be overwritten on exceeding max size or just rejected
        /// </summary>
        public bool OverwriteData;

        public List<T> Data
        {
            get
            {
                    return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(FilePath)) ?? new List<T>();
            }
        }

        public JsonFile(string path)
        {
            if (!Uri.IsWellFormedUriString(Path.GetFileNameWithoutExtension(path), UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException();
            }

            FilePath = path;
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, "[]");
            }
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
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Delete record at a given index
        /// </summary>
        /// <param name="i">Index of an element to be removed</param>
        public void Delete(int i)
        {
            List<T> buffer = Data;
            try
            {
                buffer.RemoveAt(i);
            }
            catch(Exception)
            { }

            string json = JsonConvert.SerializeObject(buffer);
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Clear file content
        /// </summary>
        public void Clear()
        {
            File.WriteAllText(FilePath, "[]");
        }
    }
}
