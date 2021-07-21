using System;
using System.Collections.Generic;
using Generator.Factories;
using GirLoader.Output.Model;
using Scriban.Runtime;

namespace Generator.Services.Writer
{
    internal class WriteClassService
    {
        private readonly WriteHelperService _writeHelperService;
        private readonly ScriptObjectFactory _scriptObjectFactory;

        public WriteClassService(WriteHelperService writeHelperService, ScriptObjectFactory scriptObjectFactory)
        {
            _writeHelperService = writeHelperService;
            _scriptObjectFactory = scriptObjectFactory;
        }

        public void Write(string projectName, string outputDir, IEnumerable<Class> classes, Namespace @namespace)
        {
            foreach (Class cls in classes)
            {
                var scriptObject = _scriptObjectFactory.CreateComplexForSymbol(@namespace, cls);

                try
                {
                    if (cls.IsFundamental)
                        WriteFundamentalClass(projectName, outputDir, cls, scriptObject);
                    else
                        WriteNonFundamentalClass(projectName, outputDir, cls, scriptObject);
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not create type {cls.Name}: {ex.Message}");
                }
            }
        }

        private void WriteNonFundamentalClass(string projectName, string outputDir, Class cls, ScriptObject scriptObject)
        {
            _writeHelperService.Write(
                projectName: projectName,
                outputDir: outputDir,
                templateName: "class.sbntxt",
                folder: Folder.Managed.Classes,
                fileName: cls.Name,
                scriptObject: scriptObject
            );

            WriteNativeClassInstance(projectName, outputDir, cls, scriptObject);
        }

        private void WriteFundamentalClass(string projectName, string outputDir, Class cls, ScriptObject scriptObject)
        {
            _writeHelperService.Write(
                projectName: projectName,
                outputDir: outputDir,
                templateName: "fundamental.class.sbntxt",
                folder: Folder.Managed.Classes,
                fileName: cls.Name,
                scriptObject: scriptObject
            );

            WriteNativeClassInstance(projectName, outputDir, cls, scriptObject);
        }

        private void WriteNativeClassInstance(string projectName, string outputDir, Class cls, ScriptObject scriptObject)
        {
            _writeHelperService.Write(
                projectName: projectName,
                outputDir: outputDir,
                templateName: "class.native.instance.sbntxt",
                folder: Folder.Native.Classes,
                fileName: cls.Name + ".Instance",
                scriptObject: scriptObject
            );
        }
    }
}
