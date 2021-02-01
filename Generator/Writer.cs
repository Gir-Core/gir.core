﻿using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Repository;
using Repository.Model;

using Scriban;

using Generator.Services;

namespace Generator
{
    public class Writer
    {
        public readonly LoadedProject Project;
        public readonly Namespace Namespace;
        public string CurrentNamespace => Namespace.Name;

        public Writer(LoadedProject project)
        {
            Project = project;
            Namespace = project.Namespace;
        }

        public IEnumerable<Task> GetAsyncTasks()
        {
            List<Task> asyncTasks = new();
            
            // As a rule of thumb, define one task per source file
            // you wish to generate. Tasks should not modify data as
            // they run asynchronously.
            asyncTasks.Add(WriteObjectFiles());
            asyncTasks.Add(WriteDelegateFiles());

            return asyncTasks;
        }

        private async Task WriteObjectFiles()
        {
            // Read generic template
            var objTemplate = ReadTemplate("object.sbntxt");
            var template = Template.Parse(objTemplate);
            
            // Create Directory
            var dir = $"output/{Project.Name}/Classes/";
            Directory.CreateDirectory(dir);
            
            // Generate a file for each class
            // TODO: We could avoid await here and return tasks instead
            foreach (Class cls in Namespace.Classes)
            {
                // Skip GObject, GInitiallyUnowned
                if (cls.NativeName == "Object" || cls.NativeName == "InitiallyUnowned")
                    continue;
                
                // GObject Class Struct
                cls.TryGetMetadata<Record>("ClassStruct", out Record classStruct);

                // These contain: Object, Signals, Fields, Native: {Properties, Methods}
                var result = await template.RenderAsync(new
                {
                    Namespace = CurrentNamespace,
                    Name = cls.ManagedName,
                    Inheritance = ObjectService.WriteInheritance(cls),
                    TypeName = cls.CType,
                    
                    ClassStruct = classStruct, // May be null
                    ClassStructName = classStruct?.ManagedName.Split('.', 2)[1],
                });

                var path = Path.Combine(dir, $"{cls.ManagedName}.Generated.cs");
                await File.WriteAllTextAsync(path, result);
            }
        }
        
        private async Task WriteDelegateFiles()
        {
            // Read generic template
            var dlgTemplate = ReadTemplate("delegate.sbntxt");
            var template = Template.Parse(dlgTemplate);
            
            // Create Directory
            var dir = $"output/{Project.Name}/Delegates/";
            Directory.CreateDirectory(dir);
            
            // Generate a file for each class
            foreach (Callback dlg in Namespace.Callbacks)
            {
                var result = await template.RenderAsync(new
                {
                    Namespace = CurrentNamespace,
                    ReturnValue = CallableService.WriteReturnValue(dlg),
                    WrapperType = dlg.NativeName,
                    WrappedType = dlg.ManagedName,
                    ManagedParameters = CallableService.WriteParameters(dlg),
                });

                var path = Path.Combine(dir, $"{dlg.ManagedName}.Generated.cs");
                await File.WriteAllTextAsync(path, result);
            }
        }

        private static string ReadTemplate(string resource)
        {
            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"Generator.Templates.{resource}");

            if (stream == null)
                throw new IOException($"Cannot get template resource file '{resource}'");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}