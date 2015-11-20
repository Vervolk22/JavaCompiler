using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis;
using SyntaxAnalysis.PH.DataTree;
using System.Reflection;
using System.Reflection.Emit;

namespace JavaToILGenerator
{
    /// <summary>
    /// Gets syntax tree as input and converts it to executable file
    /// JavaCode.exe via microsoft intermediate language and .NET reflection
    /// </summary>
    public class Generator
    {
        
        protected const string ASSEMBLY_NAME = "CompiledJava";
        protected const string MODULE_NAME = "JavaModule";
        protected const string FILE_NAME = "JavaCode.exe";

        protected AssemblyBuilder ab;
        protected SyntaxTree tree;
        
        /// <summary>
        /// Gets syntax tree and generates executable file.
        /// </summary>
        /// <param name="tree">Input syntax tree.</param>
        public void generateExe(SyntaxTree tree)
        {
            // Create executable assembly.
            this.tree = tree;
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName aName = new AssemblyName();
            aName.Name = ASSEMBLY_NAME;
            AssemblyBuilder aBuilder = domain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Save);
            ab = aBuilder;
            ModuleBuilder mBuilder = aBuilder.DefineDynamicModule(MODULE_NAME, FILE_NAME);

            // Generate type for each class in source code.
            foreach (DTreeNode<string> childNode in tree.getRootNode().Nodes)
            {
                new GeneratorTypes(aBuilder, childNode, mBuilder).generate();
            }

            // Save assembly.
            aBuilder.Save(FILE_NAME);
            Console.WriteLine("Code generation complete.");
        }
    }
}
