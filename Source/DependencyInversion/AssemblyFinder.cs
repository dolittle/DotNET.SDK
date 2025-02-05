using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Utility to discover and load assemblies installed in your application
/// </summary>
static class AssemblyFinder
{
    static readonly AssemblyLoadContextWrapper _loader = new(AssemblyLoadContext.Default);

    /// <summary>
    /// Find assemblies in the application's binary path
    /// </summary>
    /// <param name="logFailure">Take an action when an assembly file could not be loaded</param>
    /// <param name="filter">Predicate to determine assembly inclusion</param>
    /// <param name="includeExeFiles">Optionally include *.exe files</param>
    /// <returns></returns>
    public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure, Func<Assembly, bool> filter,
        bool includeExeFiles)
    {
        string path;
        try
        {
            path = AppContext.BaseDirectory;
        }
        catch (Exception)
        {
            path = Directory.GetCurrentDirectory();
        }

        return FindAssemblies(filter, path, logFailure, includeExeFiles);
    }

    /// <summary>
    /// Find assemblies in the given path
    /// </summary>
    /// <param name="filter">Predicate to determine assembly inclusion</param>
    /// <param name="assemblyPath">The path to probe for assembly files</param>
    /// <param name="logFailure">Take an action when an assembly file could not be loaded</param>
    /// <param name="includeExeFiles">Optionally include *.exe files</param>
    /// <returns></returns>
    public static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter, string assemblyPath,
        Action<string> logFailure, bool includeExeFiles)
    {
        var assemblies = FindAssemblies(assemblyPath, logFailure, includeExeFiles)
            .Where(it => filter(it.assembly))
            .OrderBy(x => x.name)
            .Select(it => it.assembly)
            .ToArray();

        return assemblies.TopologicalSort((Func<Assembly, Assembly[]>)FindDependencies, throwOnCycle: false);

        Assembly[] FindDependencies(Assembly a) => assemblies
            .Where(x => a.GetReferencedAssemblies().Any(it => it.Name == x.GetName().Name)).ToArray();
    }

    static IEnumerable<(string name, Assembly assembly)> FindAssemblies(string assemblyPath, Action<string> logFailure,
        bool includeExeFiles)
    {
        var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
        var files = dllFiles;

        if (includeExeFiles)
        {
            var exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories);
            files = dllFiles.Concat(exeFiles);
        }

        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            Assembly? assembly = null;
            string? assemblyName = null;
            try
            {
                assembly = _loader.LoadFromAssemblyName(new AssemblyName(name));
                assemblyName = assembly.GetName().Name;

            }
            catch (Exception)
            {
                try
                {
                    assembly = _loader.LoadFromAssemblyPath(file);
                    assemblyName = assembly.GetName().Name;

                }
                catch (Exception)
                {
                    logFailure(file);
                }
            }

            if (assemblyName is not null && assembly is not null)
            {
                yield return (assemblyName, assembly);
            }
        }
    }


    /// <summary>
    /// Find assembly files matching a given filter
    /// </summary>
    /// <param name="filter">Predicate to determine assembly inclusion</param>
    /// <param name="includeExeFiles"></param>
    /// <returns></returns>
    public static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool>? filter, bool includeExeFiles = false)
    {
        filter ??= _ => true;

        return FindAssemblies(_ => { }, filter, includeExeFiles: includeExeFiles);
    }
}

sealed class AssemblyLoadContextWrapper
{
    readonly AssemblyLoadContext _ctx;

    public AssemblyLoadContextWrapper(AssemblyLoadContext ctx)
    {
        _ctx = ctx;
    }

    public Assembly LoadFromStream(Stream assembly)
    {
        return _ctx.LoadFromStream(assembly);
    }

    public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
    {
        return _ctx.LoadFromAssemblyName(assemblyName);
    }

    public Assembly LoadFromAssemblyPath(string assemblyName)
    {
        return _ctx.LoadFromAssemblyPath(assemblyName);
    }
}

static class TopologicalSortExtensions
{
    /// <summary>
    /// Performs a topological sort on the enumeration based on dependencies
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dependencies"></param>
    /// <param name="throwOnCycle"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies,
        bool throwOnCycle = true)
    {
        var sorted = new List<T>();
        var visited = new HashSet<T>();

        foreach (var item in source)
        {
            Visit(item, visited, sorted, dependencies, throwOnCycle);
        }

        return sorted;
    }

    static void Visit<T>(T item, ISet<T> visited, ICollection<T> sorted, Func<T, IEnumerable<T>> dependencies,
        bool throwOnCycle)
    {
        if (!visited.Add(item))
        {
            if (throwOnCycle && !sorted.Contains(item))
            {
                throw new CyclicDependencyFound("Cyclic dependency found for " + item);
            }
        }
        else
        {
            foreach (var dep in dependencies(item))
            {
                Visit(dep, visited, sorted, dependencies, throwOnCycle);
            }

            sorted.Add(item);
        }
    }
}

/// <summary>
/// 
/// </summary>
public class CyclicDependencyFound(string message) : Exception(message);
