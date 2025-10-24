using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppImageLauncher.Classes;
using LiteDB;
namespace AppImageLauncher.Controller;

public class Cache
{
    public static async Task WriteCache(Application newApp)
    {
        try
        {
            var mapper = BsonMapper.Global;
            mapper.RegisterType<TimeSpan>
            (
                serialize: t => t.ToString(),    
                deserialize: bson => TimeSpan.Parse(bson.AsString)
            );
            
            var db = new LiteDatabase(Paths.DirectoryStructure, mapper);
            var col = db.GetCollection<Application>("applications");
            var maxId = col.FindAll()
                .OrderByDescending(x => x.Id)
                .FirstOrDefault()?.Id ?? 0;
            newApp.Id = maxId + 1;
            col.Insert(newApp);
            db.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task<ObservableCollection<Application>> ReadCache()
    {
        try
        {
            var mapper = BsonMapper.Global;
            mapper.RegisterType<TimeSpan>
            (
                serialize: t => t.ToString(),    
                deserialize: bson => TimeSpan.Parse(bson.AsString)
            );
            
            var db = new LiteDatabase(Paths.DirectoryStructure, mapper);
            var col = db.GetCollection<Application>("applications");
            var apps = col.FindAll().ToList(); 
            db.Dispose();
            return new ObservableCollection<Application>(apps);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static ApplicationConfiguration ReadConfig()
    {
        var directoryStructure = File.ReadAllText(Paths.ConfigJson);
        var doc = JsonDocument.Parse(directoryStructure);
        var themeValue = doc.RootElement.GetProperty("Theme").GetString();

        var categoriesJson = doc.RootElement.GetProperty("Categories");
        List<string?> categories = categoriesJson.EnumerateArray()
            .Select(x => x.GetString())
            .ToList();

        var appConfig = new ApplicationConfiguration
        {
            Theme = themeValue,
            Categories = categories
        };
        return appConfig;
    }

    public static async Task UpdateRunTime(Application runningApp)
    {
        Console.WriteLine($"Updating runtime to {runningApp.RunTime}");

        var mapper = BsonMapper.Global;
        mapper.RegisterType<TimeSpan>
        (
            serialize: t => t.ToString(),    
            deserialize: bson => TimeSpan.Parse(bson.AsString)
        );

        using var db = new LiteDatabase(Paths.DirectoryStructure, mapper);
        var col = db.GetCollection<Application>("applications");
        var existingApp = col.FindOne(x => x.Name == runningApp.Name);
        if (existingApp != null)
        {
            existingApp.RunTime += runningApp.RunTime;
            col.Update(existingApp);
        }
        await Task.CompletedTask;
    }

    public static async Task UpdateApplication(Application app)
    {
        Console.WriteLine($"Updating application to {app.Id}");
        try
        {
            using var db = new LiteDatabase(Paths.DirectoryStructure);
            var col = db.GetCollection<Application>("applications");
            var existingApp = col.FindById(app.Id);
            if (existingApp != null)
            {
                Console.WriteLine($"FOUND: {existingApp.Name}, ID: {existingApp.Id}");

                var success = col.Update(app);
                Console.WriteLine($"UPDATE STATE: {success}");
            }
            else
            {
                Console.WriteLine("COULD NOT FIND APP");
            }
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}