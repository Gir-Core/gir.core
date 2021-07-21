using System;
using System.Collections.Generic;
using Generator.Factories;
using GirLoader.Output.Model;
using Scriban.Runtime;

namespace Generator.Services.Writer
{
    internal class WriteUnionsService
    {
        private readonly WriteHelperService _writeHelperService;
        private readonly ScriptObjectFactory _scriptObjectFactory;

        public WriteUnionsService(WriteHelperService writeHelperService, ScriptObjectFactory scriptObjectFactory)
        {
            _writeHelperService = writeHelperService;
            _scriptObjectFactory = scriptObjectFactory;
        }

        public void Write(string projectName, string outputDir, IEnumerable<Union> unions, Namespace @namespace)
        {
            foreach (var union in unions)
            {
                try
                {
                    var scriptObject = _scriptObjectFactory.CreateComplexForSymbol(@namespace, union);

                    var name = union.Metadata["Name"]?.ToString() ?? throw new Exception("Union is missing it's name");

                    _writeHelperService.Write(
                        projectName: projectName,
                        outputDir: outputDir,
                        templateName: "union.sbntxt",
                        folder: Folder.Managed.Records,
                        fileName: name,
                        scriptObject: scriptObject
                    );

                    _writeHelperService.Write(
                        projectName: projectName,
                        outputDir: outputDir,
                        templateName: "union.native.sbntxt",
                        folder: Folder.Native.Records,
                        fileName: name,
                        scriptObject: scriptObject
                    );
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not write union for {union.Name}: {ex.Message}");
                }
            }
        }
    }
}
