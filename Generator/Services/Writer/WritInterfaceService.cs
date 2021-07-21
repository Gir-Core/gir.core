using System;
using System.Collections.Generic;
using Generator.Factories;
using GirLoader.Output.Model;
using Scriban.Runtime;

namespace Generator.Services.Writer
{
    internal class WriteInterfaceService
    {
        private readonly WriteHelperService _writeHelperService;
        private readonly ScriptObjectFactory _scriptObjectFactory;

        public WriteInterfaceService(WriteHelperService writeHelperService, ScriptObjectFactory scriptObjectFactory)
        {
            _writeHelperService = writeHelperService;
            _scriptObjectFactory = scriptObjectFactory;
        }

        public void Write(string projectName, string outputDir, IEnumerable<Interface> interfaces, Namespace @namespace)
        {
            foreach (var iface in interfaces)
            {
                try
                {
                    var scriptObject = _scriptObjectFactory.CreateComplexForSymbol(@namespace, iface);

                    _writeHelperService.Write(
                        projectName: projectName,
                        outputDir: outputDir,
                        templateName: "interface.sbntxt",
                        folder: Folder.Managed.Interfaces,
                        fileName: iface.Name,
                        scriptObject: scriptObject
                    );

                    _writeHelperService.Write(
                        projectName: projectName,
                        outputDir: outputDir,
                        templateName: "interface.native.instance.sbntxt",
                        folder: Folder.Native.Interfaces,
                        fileName: iface.Name + ".Instance",
                        scriptObject: scriptObject
                    );
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not write interface for {iface.Name}: {ex.Message}");
                }
            }
        }
    }
}
