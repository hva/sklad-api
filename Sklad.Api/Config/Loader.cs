﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Sklad.Api.Config
{
    public class Loader
    {
        public static ApiConfig Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            using (var stream = File.OpenRead(path))
            {
                var serializer = new DataContractJsonSerializer(typeof(ApiConfig));
                return (ApiConfig)serializer.ReadObject(stream);
            }
        }
    }
}
