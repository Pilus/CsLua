﻿namespace CsLuaConverter.Providers.TypeProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CsLuaConverter.Providers.TypeProvider.TypeCollections;

    using CsLuaFramework;
    using CsLuaFramework.Wrapping;
    using Microsoft.CodeAnalysis;
    using TypeKnowledgeRegistry;

    public class TypeNameProvider : ITypeProvider
    {
        private readonly LoadedNamespace rootNamespace;
        private List<LoadedNamespace> refenrecedNamespaces;
        private List<LoadedNamespace> currentNamespaces;

        private readonly List<NativeTypeResult> predefinedNativeTypeResults = new List<NativeTypeResult>()
        {
            new NativeTypeResult("int", typeof(int)),
            new NativeTypeResult("object", typeof(object)),
            new NativeTypeResult("string", typeof(string)),
            new NativeTypeResult("bool", typeof(bool)),
            new NativeTypeResult("long", typeof(long)),
            new NativeTypeResult("double", typeof(double)),
            new NativeTypeResult("float", typeof(float)),
            new NativeTypeResult("void", typeof(void)),
        };

        public TypeNameProvider(IEnumerable<BaseTypeCollection> typeCollections)
        {
            this.rootNamespace = new LoadedNamespace(null);

            foreach (var typeCollection in typeCollections)
            {
                foreach (var type in typeCollection)
                {
                    this.LoadType(type);
                }
            }
        }

        private void LoadType(Type type)
        {
            var nameParts = StripGenerics(type.FullName).Split('.');

            if (nameParts.Length < 2)
            {
                return;
            }

            LoadedNamespace currentNamespace = null;
            foreach (var namePart in nameParts.Take(nameParts.Length - 1))
            {
                currentNamespace = (currentNamespace ?? this.rootNamespace).Upsert(namePart);
            }

            if (currentNamespace == null)
            {
                throw new ProviderException("No namespace found.");
            }
            currentNamespace.Upsert(type);
        }

        public void ClearNamespaces()
        {
            this.currentNamespaces = new List<LoadedNamespace>() {this.rootNamespace};
            this.refenrecedNamespaces = new List<LoadedNamespace>() {this.rootNamespace};
        }

        public void AddNamespace(string[] namespaceName)
        {
            var found = false;
            foreach (var refenrecedNamespace in this.currentNamespaces)
            {
                var loadedNamespace = refenrecedNamespace.TryGetNamespace(namespaceName);
                if (loadedNamespace != null)
                {
                    this.refenrecedNamespaces.Add(loadedNamespace);
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                throw new ProviderException($"Could not find namespace: {string.Join(".",namespaceName)}.");
            }
        }

        public void SetCurrentNamespace(string[] currentNamespace)
        {
            for (var i = currentNamespace.Count(); i >= 1; i--)
            {
                var ns = this.rootNamespace.TryGetNamespace(currentNamespace.Take(i).ToList());
                this.refenrecedNamespaces.Add(ns);
                this.currentNamespaces.Add(ns);
            }
        }

        public ITypeResult LookupType(IEnumerable<string> names)
        {
            var type = this.TryLookupTypeWithGenerics(names, null);
            if (type == null)
            {
                throw new ProviderException(string.Format("Could not find type '{0}' in the referenced namespaces.",
                    string.Join(".", names)));
            }

            return type;
        }

        public ITypeResult LookupType(IEnumerable<string> names, int numGenerics)
        {
            var type = this.TryLookupTypeWithGenerics(names, numGenerics);
            if (type == null)
            {
                throw new ProviderException(string.Format("Could not find type '{0}' in the referenced namespaces.",
                    string.Join(".", names)));
            }

            return type;
        }


        public ITypeResult LookupType(string name)
        {
            return this.LookupTypeWithGenerics(name, null);
        }

        public ITypeResult TryLookupType(string name, int? numGenerics)
        {
            if (name == "var" || name == "void")
            {
                return null;
            }

            var nativeType = this.predefinedNativeTypeResults.FirstOrDefault(t => name.Equals(t.NativeName));
            if (nativeType != null)
            {
                return nativeType;
            }

            var nameWithoutGenerics = StripGenerics(name);
            foreach (var refenrecedNamespace in this.refenrecedNamespaces)
            {
                if (refenrecedNamespace.Types.ContainsKey(nameWithoutGenerics))
                {
                    var types = refenrecedNamespace.Types[nameWithoutGenerics];
                    foreach (var e in types)
                    {
                        var type = e.GetTypeResult();

                        if (numGenerics == null || numGenerics == type.NumGenerics)
                        {
                            return type;
                        }
                    }
                }
            }

            return null;
        }

        public ITypeResult TryLookupType(IEnumerable<string> names, int? numGenerics)
        {
            return this.TryLookupTypeWithGenerics(names, null);
        }

        private ITypeResult LookupTypeWithGenerics(string name, int? numGenerics)
        {
            var type = this.TryLookupType(name, numGenerics);

            if (type != null)
            {
                return type;
            }

            throw new ProviderException(string.Format("Could not find type '{0}' in the referenced namespaces.", name));
        }

        private ITypeResult TryLookupTypeWithGenerics(IEnumerable<string> names, int? numGenerics)
        {
            if (names.Count() == 1)
            {
                return this.LookupTypeWithGenerics(names.Single(), numGenerics);
            }

            var nameWithoutGenerics = StripGenerics(names.First());
            foreach (var refenrecedNamespace in this.refenrecedNamespaces)
            {
                if (refenrecedNamespace.Types.ContainsKey(nameWithoutGenerics))
                {
                    return refenrecedNamespace.Types[nameWithoutGenerics].Single().GetTypeResult(string.Join(".", names.Skip(1)));
                }
                
                if (names.Count() > 1 && refenrecedNamespace.SubNamespaces.ContainsKey(nameWithoutGenerics))
                {
                    var current = refenrecedNamespace.SubNamespaces[nameWithoutGenerics];
                    var remainingNames = names.Skip(1);
                    
                    while (remainingNames.Any())
                    {
                        var name = StripGenerics(remainingNames.First());
                        remainingNames = remainingNames.Skip(1);

                        if (current.Types.ContainsKey(name))
                        {
                            var types = current.Types[name];
                            foreach (var e in types)
                            {
                                var type = e.GetTypeResult(string.Join(".", remainingNames));

                                if (numGenerics == null || numGenerics == type.NumGenerics)
                                {
                                    return type;
                                }
                            }
                        }

                        if (current.SubNamespaces.ContainsKey(name))
                        {
                            current = current.SubNamespaces[name];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return null;
        }

        public MethodKnowledge[] GetExtensionMethods(Type type, string name)
        {
            return this.refenrecedNamespaces.SelectMany(ns => ns.GetExtensionMethods(type, name)).ToArray();
        }

        private static string StripGenerics(string name)
        {
            return name.Split('`').First();
        }


        

    }
}