﻿using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace LaptopMonitorLibrary
{
    public class JsonFile<T>
    {
        /// <summary>
        /// Path to .json file
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// Whether data should be overwritten on exceeding max size or just rejected
        /// </summary>
        public bool OverwriteData;

        public List<T> Data
        {
            get
            {
                    return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(Path)) ?? new List<T>();
            }
        }

        public JsonFile(string path, int maxSize = -1)
        {
            Path = path;
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, "[]");
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
            File.WriteAllText(Path, json);
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
