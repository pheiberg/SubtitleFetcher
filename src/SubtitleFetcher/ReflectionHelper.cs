using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SubtitleFetcher
{
    public class ReflectionHelper
    {
        public static IEnumerable<Type> GetAllConcreteImplementors<T>()
        {
            var assemblies = GetAllAssemblies();
            var type = typeof(T);
            var types = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(implementor => type.IsAssignableFrom(implementor) && !implementor.IsInterface && !implementor.IsAbstract)
                .OrderBy(implementor => implementor.Name);
            return types;
        }

        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            var assemblies = AssembliesFromPath(AppDomain.CurrentDomain.BaseDirectory);
            string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (!Directory.Exists(privateBinPath))
                return assemblies;

            return assemblies.Concat(AssembliesFromPath(privateBinPath));
        }

        public static IEnumerable<Assembly> AssembliesFromPath(string path)
        {
            IList<Assembly> assemblies = new List<Assembly>();
            foreach (string assemblyFile in Directory.GetFiles(path).Where(file => Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(assemblyFile);
                    assemblies.Add(assembly);
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch
                    // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }
            return assemblies;
        }
    }
}