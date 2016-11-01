﻿namespace CsLuaConverter.MethodSignature
{
    using System.Collections.Generic;
    using System.Linq;

    public class SignatureComposer<T>
    {
        private static readonly int[] Primes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997 };

        private readonly ISemanticAdaptor<T> semanticAdaptor;

        public SignatureComposer(ISemanticAdaptor<T> semanticAdaptor)
        {
            this.semanticAdaptor = semanticAdaptor;
        }

        public SignatureComponent<T>[] GetSignatureComponents(T[] inputTypes)
        {
            var components = new List<SignatureComponent<T>>();
            for (var i = 0; i < inputTypes.Length; i++)
            {
                var inputType = inputTypes[i];
                this.AddSignatureComponents(Primes[i], inputType, components);
            }

            return components.ToArray();
        }

        public SignatureComponent<T>[] GetSignatureComponents(T type)
        {
            var components = new List<SignatureComponent<T>>();
            this.AddSignatureComponents(1, type, components);

            return components.ToArray();
        }

        private void AddSignatureComponents(int coefficient, T type, List<SignatureComponent<T>> components)
        {
            if (this.semanticAdaptor.IsArray(type))
            {
                this.AddSignatureComponents(coefficient * 3, this.semanticAdaptor.GetArrayGeneric(type), components);
                return;
            }

            if (this.semanticAdaptor.IsGenericType(type))
            {
                if (this.semanticAdaptor.IsMethodGeneric(type))
                {
                    components.Add(new SignatureComponent<T>(coefficient, 1));
                }
                else
                {
                    components.Add(new SignatureComponent<T>(coefficient, type));
                }

                return;
            }

            var generics = this.semanticAdaptor.GetGenerics(type);
            
            if (generics.Any())
            {
                var hash = GetSignatureHash(this.semanticAdaptor.GetName(type));
                var subCoefficient = coefficient * hash;

                for (var i = 0; i < generics.Length; i++)
                {
                    this.AddSignatureComponents(subCoefficient * Primes[i], generics[i], components);
                }

                return;
            }

            components.Add(new SignatureComponent<T>(coefficient, GetSignatureHash(this.semanticAdaptor.GetName(type))));
        }

        private static int GetSignatureHash(string name)
        {
            var value = 0;
            var chars = name.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                value = value + (chars[i] * Primes[i]);
            }

            return value;
        }
    }
}