using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Generator.Services;
using Generator.Services.Writer;
using GirLoader;
using GirLoader.Output.Model;
using StrongInject;

namespace Generator
{
    public class Generator
    {
        public string OutputDir { get; init; } = "output";
        public bool UseAsync { get; init; } = true;
        public bool GenerateDocComments { get; init; } = false;

        public void Write(IEnumerable<GirFile> projects)
        {
            //Loader.EnableVerboseOutput(); //TODO: Configure via settings
            var repositories = Loader.Load(FileResolver.ResolveFile, projects).ToList();

            var typeRenamer = new TypeRenamer();
            typeRenamer.SetMetadata(repositories.Select(x => x.Namespace));
            typeRenamer.FixClassNameClashes(repositories.Select(x => x.Namespace));

            WriterService writerService = new Container().Resolve().Value;

            // Set writer options
            var options = new WriterOptions
            {
                GenerateDocComments = GenerateDocComments,
            };

            Log.Information("Optional Writer Properties:");
            Log.Information($" - Generating LGPL Documentation (not implemented yet): {options.GenerateDocComments}");

            Log.Information("Ready to write.");

            if (UseAsync)
            {
                Parallel.ForEach(repositories,
                    proj => writerService.Write(proj.Namespace, OutputDir, options));
            }
            else
            {
                Log.Warning("Async Generation is disabled. Generation may be slower than normal.");

                // Disable asynchronous writing for an easier debugging experience
                foreach (Namespace ns in repositories.Select(x => x.Namespace))
                    writerService.Write(ns, OutputDir, options);
            }

            Log.Information("Writing completed successfully");
        }
    }
}
