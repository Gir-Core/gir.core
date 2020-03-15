﻿using static Bullseye.Targets;
using static SimpleExec.Command;
using static Targets;
using static Commands;
using static Projects;

class Program
{

    static void Main(string[] args)
    {
        Target(generate_wrapper, () => {
            GenerateProject(CWRAPPER);
            GenerateProject(GLIB_WRAPPER);
            GenerateProject(CAIRO_WRAPPER);
            GenerateProject(XLIB_WRAPPER);
            GenerateProject(PANGO_WRAPPER);
            GenerateProject(GDK_WRAPPER);
            GenerateProject(GIO_WRAPPER);
            GenerateProject(GOBJECT_WRAPPER);
            GenerateProject(GDK_PIXBUF_WRAPPER);
            GenerateProject(GTK_WRAPPER);
        });

        Target(build_gtk_core, DependsOn(generate_wrapper), () => {
            Run(dotnet, $"{build} {GTK_CORE}");
        });

        Target(build_webkitgtk_core, DependsOn(generate_wrapper), () => {
            GenerateProject(WEBKITGTK_WRAPPER);
            GenerateProject(JAVASCRIPT_CORE_WRAPPER);
            
            Run(dotnet, $"{build} {WEBKITGTK_CORE}");
        });

        Target(build_webkit2webextensions_core, () => {
            GenerateProject(WEBKIT2WEBEXTENSION_WRAPPER);
            Run(dotnet, $"{build} {WEBKIT2WEBEXTENSION_CORE}");
        });

        Target("default", DependsOn(build_gtk_core, build_webkitgtk_core, build_webkit2webextensions_core));
        RunTargetsAndExit(args);
    }

    private static void GenerateProject(string path)
    {
        path += "Generate/";

        Run(dotnet, $"{build} {path}");
        Run(dotnet, $"{run} {path}", path);
    }
}