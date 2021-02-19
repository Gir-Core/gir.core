﻿using System;
using System.Collections.Generic;
using Repository.Model;
using Scriban.Runtime;

namespace Generator.Services.Writer
{
    internal class WriteSymbolsService
    {
        private readonly WriteHelperService _writeHelperService;

        public WriteSymbolsService(WriteHelperService writeHelperService)
        {
            _writeHelperService = writeHelperService;
        }

        public void WriteSymbols(string projectName, string outputDir, string templateName, string subfolder, string name, IEnumerable<Symbol> symbols, Namespace @namespace)
        {
            var scriptObject = new ScriptObject
            {
                {name.ToLower(), symbols}, 
                {"namespace", @namespace}
            };
            scriptObject.Import("write_native_constant", new Func<Constant, string>(TemplateWriter.WriteNativeConstant));
            scriptObject.Import("write_native_method", new Func<Method, string>(TemplateWriter.WriteNativeMethod));

            try
            {
                _writeHelperService.Write(
                    projectName: projectName,
                    templateName: templateName,
                    folder: subfolder,
                    outputDir: outputDir,
                    fileName: name,
                    scriptObject: scriptObject
                );
            }
            catch (Exception ex)
            {
                Log.Error($"Could not write symbols for {@namespace.Name} / {name}: {ex.Message}");
            }
        }
    }
}
