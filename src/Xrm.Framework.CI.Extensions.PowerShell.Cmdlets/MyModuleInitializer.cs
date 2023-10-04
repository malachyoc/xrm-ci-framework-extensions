using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Framework.CI.Extensions.PowerShell.Cmdlets
{
    // Register the event handler as early as you can in your code.
    // A good option is to use the IModuleAssemblyInitializer interface
    // that PowerShell provides to run code early on when your module is loaded.

    // This class will be instantiated on module import and the OnImport() method run.
    // Make sure that:
    //  - the class is public
    //  - the class has a public, parameterless constructor
    //  - the class implements IModuleAssemblyInitializer
    public class MyModuleInitializer : IModuleAssemblyInitializer
    {
        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += DependencyResolution.ResolveNewtonsoftJson;
        }
    }

    // Clean up the event handler when the the module is removed
    // to prevent memory leaks.
    //
    // Like IModuleAssemblyInitializer, IModuleAssemblyCleanup allows
    // you to register code to run when a module is removed (with Remove-Module).
    // Make sure it is also public with a public parameterless contructor
    // and implements IModuleAssemblyCleanup.
    //public class MyModuleCleanup : IModuleAssemblyCleanup
    //{
    //    //Commented out as method does not exist on PS4
    //    public void OnRemove(PSModuleInfo psModuleInfo)
    //    {
    //        AppDomain.CurrentDomain.AssemblyResolve -= DependencyResolution.ResolveNewtonsoftJson;
    //    }
    //}

    internal static class DependencyResolution
    {
        private static readonly string s_modulePath = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location);

        public static Assembly ResolveNewtonsoftJson(object sender, ResolveEventArgs args)
        {
            // Parse the assembly name
            var assemblyName = new AssemblyName(args.Name);

            // We only want to handle the dependency we care about.
            // In this example it's Newtonsoft.Json.
            if (!assemblyName.Name.Equals("Newtonsoft.Json"))
            {
                return null;
            }

            // Generally the version of the dependency you want to load is the higher one,
            // since it's the most likely to be compatible with all dependent assemblies.
            // The logic here assumes our module always has the version we want to load.
            // Also note the use of Assembly.LoadFrom() here rather than Assembly.LoadFile().
            return Assembly.LoadFrom(Path.Combine(s_modulePath, "Newtonsoft.Json.dll"));
        }
    }
}
