using System;
using System.Collections.Generic;
using Generator.Factories;
using GirLoader.Output.Model;
using Scriban.Runtime;

namespace Generator.Services.Writer
{
    internal class WriteRecordsService
    {
        private readonly WriteHelperService _writeHelperService;
        private readonly ScriptObjectFactory _scriptObjectFactory;

        public WriteRecordsService(WriteHelperService writeHelperService, ScriptObjectFactory scriptObjectFactory)
        {
            _writeHelperService = writeHelperService;
            _scriptObjectFactory = scriptObjectFactory;
        }

        public void Write(string projectName, string outputDir, IEnumerable<Record> records, Namespace @namespace)
        {
            foreach (var record in records)
            {
                try
                {
                    var scriptObject = _scriptObjectFactory.CreateComplexForSymbol(@namespace, record);

                    var name = record.Metadata["Name"]?.ToString() ?? throw new Exception("Record is missing it's name");

                    if (record.GLibClassStructFor is null)
                    {
                        //Regular struct
                        _writeHelperService.Write(
                            projectName: projectName,
                            outputDir: outputDir,
                            templateName: "record.sbntxt",
                            folder: Folder.Managed.Records,
                            fileName: name,
                            scriptObject: scriptObject
                        );
                    }

                    _writeHelperService.Write(
                        projectName: projectName,
                        outputDir: outputDir,
                        templateName: GetNativeTemplateName(record),
                        folder: GetNativeFolder(record),
                        fileName: name,
                        scriptObject: scriptObject
                    );
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not write record for {record.Name}: {ex.Message}");
                }
            }
        }

        private string GetNativeFolder(Record record)
            => record.GLibClassStructFor?.GetResolvedType() switch
            {
                Interface => Folder.Native.Interfaces,
                Class => Folder.Native.Classes,
                _ => Folder.Native.Records
            };

        private string GetNativeTemplateName(Record record)
            => record.GLibClassStructFor?.GetResolvedType() switch
            {
                Interface => "record.native.interface.sbntxt",
                Class => "record.native.class.sbntxt",
                _ => "record.native.sbntxt",
            };
    }
}
