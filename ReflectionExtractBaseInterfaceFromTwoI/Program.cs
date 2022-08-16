using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionExtractBaseInterfaceFromTwoI
{
    static class Program
    {
        class cmp : System.Collections.Generic.IEqualityComparer<MethodInfo>
        {
            bool compareParams(ParameterInfo[] xP, ParameterInfo[] yP)
            {
                if (xP.Length != yP.Length) return false;
                for (int i = 0; i < yP.Length; i++)
                    if (xP[i].ParameterType != yP[i].ParameterType)
                        return false;
                return true;
            }

            public bool Equals(MethodInfo x, MethodInfo y) =>
                (x.Name == y.Name)
                && (x.ReturnType == y.ReturnType)
                && compareParams(x.GetParameters(), y.GetParameters());


            public int GetHashCode(MethodInfo obj) => obj.ToString().GetHashCode();
        }
        public static string CSharpName(this Type type)
        {
            var sb = new System.Text.StringBuilder();
            var name = type.Name;
            if (!type.IsGenericType) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                                            .Select(t => CSharpName(t))));
            sb.Append(">");
            return sb.ToString();
        }
        static void Main(string[] args)
        {
            string path = @"c:\Users\shmykovad\source\repos\ReflectionExtractBaseInterfaceFromTwoI\ReflectionExtractBaseInterfaceFromTwoI\bin\Debug\CoreDefs.dll";
            var DLL = Assembly.LoadFile(path);
            var allTs = DLL.GetExportedTypes();
            var ts = allTs.Where(t => t.FullName.Contains("ILoanContractsManager") || t.FullName.Contains("ICXLoanContractsManager")).ToList();


            var methods = ts.Select(x => x.GetMethods()).Aggregate((x, y) => x.Intersect(y, new cmp()).ToArray());
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo m = methods[i];
                string q = (i < methods.Length - 1) ? "," : "";
                Console.WriteLine($" {CSharpName(m.ReturnType)}    {m.Name}( {m.GetParameters().Aggregate("", (a, p) => a + (p.IsOut ? " out " : p.IsIn ? " in " : "") + $"{p.ParameterType.CSharpName()} {p.Name} ,").TrimEnd(',')}) ");
            }
            var ch = Console.ReadLine();
        }
    }
}