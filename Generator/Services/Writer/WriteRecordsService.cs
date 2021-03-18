﻿using System;
using System.Collections.Generic;
using Generator.Factories;
using Repository.Model;
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
                    var scriptObject =  _scriptObjectFactory.CreateComplex(@namespace);
                    scriptObject.Import(record);
                    //TODO: Workaround as long as scriban indexer are broken see https://github.com/scriban/scriban/issues/333
                    scriptObject.Import("get_metadata", new Func<string, object?>(key => record.Metadata[key]));

                    
                    _writeHelperService.Write(
                        projectName: projectName,
                        outputDir: outputDir,
                        templateName: record.IsClassStruct ? "classstruct.sbntxt" : "struct.sbntxt",
                        folder: record.IsClassStruct ? "Classes" : "Records",
                        fileName: record.SymbolName,
                        scriptObject: scriptObject
                    );
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not write record for {record.SymbolName}: {ex.Message}");
                }
            }
        }
    }
}
