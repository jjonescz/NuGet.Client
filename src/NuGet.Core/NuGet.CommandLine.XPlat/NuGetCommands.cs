// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#nullable enable

using System;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.CommandLine.XPlat.Commands.Package.PackageDownload;
using NuGet.CommandLine.XPlat.Commands.Package.Update;

namespace NuGet.CommandLine.XPlat;

/// <summary>
/// Class used by the .NET SDK to register NuGet's commands into the dotnet CLI.
/// </summary>
public static class NuGetCommands
{
    /// <summary>
    /// <para>Adds NuGet's dotnet CLI commands to the dotnet CLI RootCommand object</para>
    /// </summary>
    /// <param name="rootCommand">The CLI's RootCommand instance</param>
    /// <param name="interactiveOption">The .NET SDK has code to detect when output is redirected or </param>
    /// <param name="handler">Handler for commands that need virtual project handling. The parameter is the inner command handler which allows this outer handler to do any work before or after the inner handler.</param>
    /// <remarks>Many of NuGet's commands are defined in the dotnet/sdk repo, and those run NuGet.CommandLine.XPlat.dll as a child process.
    /// Those commands are not added by this method.</remarks>
    public static void Add(RootCommand rootCommand, Option<bool> interactiveOption, Func<Func<IVirtualProjectBuilder?, CancellationToken, Task<int>>, CancellationToken, Task<int>>? handler = null)
    {
        var packageCommand = rootCommand.Subcommands.FirstOrDefault(c => c.Name == "package");
        if (packageCommand is null)
        {
            packageCommand = new Command("package");
            rootCommand.Subcommands.Add(packageCommand);
        }

        PackageUpdateCommand.Register(packageCommand, interactiveOption, handler);
        PackageDownloadCommand.Register(packageCommand, interactiveOption);
    }

    // For binary backcompat. To delete once the SDK starts using the first overload.
    public static void Add(RootCommand rootCommand, Option<bool> interactiveOption)
    {
        Add(rootCommand, interactiveOption, handler: null);
    }

    // To delete once the SDK starts using the first overload. Joys of public APIs.
    public static void Add(RootCommand rootCommand)
    {
        var interactiveOption = new Option<bool>("--interactive")
        {
            Description = Strings.AddPkg_InteractiveDescription,
            DefaultValueFactory = _ => Console.IsOutputRedirected
        };
        Add(rootCommand, interactiveOption);
    }
}
