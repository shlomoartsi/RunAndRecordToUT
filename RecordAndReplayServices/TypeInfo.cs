using System;

namespace RecordAndReplayServices
{
    public readonly struct TypeInfo
    {
        [Flags]
        public enum TypeCompareOptions
        {
            /// <summary>
            /// Compare all fields
            /// </summary>
            CompareAll = 0,
            //Compare all fields with simplified assembly that ignores version and culture
            CompareWithSimplifiedAssembly = 0b0001,
            //Ignore out parameter
            IgnoreOut = 0b0010,
            //ignore ref parameter
            IgnoreRef = 0b0100,
            //Compare all fields with simplified assembly that ignores version and culture and ignore out parameter
            CompareSimplified = CompareWithSimplifiedAssembly | IgnoreOut | IgnoreRef
        }

        public TypeInfo(string assemblyName,string @namespace,string name)
        {
            AssemblyName = assemblyName;
            Namespace = @namespace;
            Name = name;
        }

        public string AssemblyName { get; }
        public string Namespace { get; }
        public string Name { get; }
        public string FullName => Namespace + "." + Name;

        public override string ToString()
        {
            return AssemblyName + ", " + FullName;
        }

        public bool Equals(TypeInfo other, 
            TypeCompareOptions compareOptions = TypeCompareOptions.CompareSimplified)
        {
            if (this.Namespace != other.Namespace || this.Name.Replace('&',' ').TrimEnd() != other.Name) return false;

            if (this.AssemblyName == other.AssemblyName) return true;

            switch (compareOptions)
            {
                case TypeCompareOptions.CompareAll:
                    return false;
                case TypeCompareOptions.CompareWithSimplifiedAssembly:
                    if (!TryGetSimplifiedNameSpace(this.AssemblyName, out var simplifiedAssembly)) return false;
                    if (!TryGetSimplifiedNameSpace(other.AssemblyName, out var otherAssembly)) return false;
                    return simplifiedAssembly == otherAssembly;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareOptions), compareOptions, null);
            }
        }

        private static bool TryGetSimplifiedNameSpace(string assemblyName,out string simplifiedAssembly)
        {
            simplifiedAssembly = null;
            var firstIndexOfComma = assemblyName.IndexOf(',');
            if (firstIndexOfComma == -1) return false;
            simplifiedAssembly = assemblyName.Substring(0, firstIndexOfComma);
            return true;
        }

        

        public bool TryGetType(out Type type)
        {
            type = null;

            //try get type of 
            //"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
            var typeWithNs = Namespace + "." + Name;

            //first try specific version type and later non version dependent
            try
            {
                var typeResult = Type.GetType(typeWithNs + ", " + AssemblyName);
                if (typeResult != null)
                {
                    type = typeResult;
                    return true;
                }
            }
            catch
            {
                //Ignore and try another way to load type
            }

            try
            {
                if (!TryGetSimplifiedNameSpace(AssemblyName, out var fullTypeNoVersion)) return false;
                var typeResult = Type.GetType(typeWithNs + ", " + fullTypeNoVersion);
                if (typeResult == null) return false;
                type = typeResult;
                return true;
            }
            catch 
            {
                //ignore
            }

            return false;
        }

    }
}