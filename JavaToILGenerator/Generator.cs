using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace JavaToILGenerator
{
    public class Generator
    {
        public void GenerateExe()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName aName = new AssemblyName();
            aName.Name = "CompiledJava";
            AssemblyBuilder aBuilder = domain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Save);
            ModuleBuilder mBuilder = aBuilder.DefineDynamicModule("JavaModule", "JavaCode.exe");

        }
    }
}
