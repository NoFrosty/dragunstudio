using Evopeons.Common;
using Evopeons.Server.Models;
using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.Server.Services
{
    public class DataService : IDisposable
    {
        readonly LiteDatabase database;

        const string collectonNameFiles = "Files";
        const string collectonNameGeneric = "Generic";


        public DataService(IOptions<AppSettings> appSettings)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(appSettings.Value.DatabaseFile));
            database = new LiteDatabase(appSettings.Value.DatabaseFile);
            database.GetCollection<Common.FileInfo>(collectonNameFiles);
        }

        private void SetGenericReferenceValue<T>(string key, T value) where T : class, new()
        {
            var collection = database.GetCollection(collectonNameGeneric);
            collection.Upsert(key, database.Mapper.ToDocument(value));
        }

        public T GetGenericReferenceValue<T>(string key) where T : class, new()
        {
            var collection = database.GetCollection(collectonNameGeneric);
            var document = collection.FindById(key);
            return document == null ? new T() : database.Mapper.ToObject<T>(document);
        }

        public void SetGenericValue(string key, string value)
        {
            var genericValue = new GenericValue()
            {
                Value = value,
                Id = key,
            };
            var collection = database.GetCollection<GenericValue>(collectonNameGeneric);
            collection.Upsert(key, genericValue);
        }

        public string GetGenericValue(string key)
        {
            var collection = database.GetCollection<GenericValue>(collectonNameGeneric);
            return collection.FindById(key)?.Value;
        }

        public void SetVersion(SemVersion version) => SetGenericReferenceValue("Version", version);
        public SemVersion GetVersion() => GetGenericReferenceValue<SemVersion>("Version");


        public void Dispose()
        {
            database.Dispose();
        }
    }
}
