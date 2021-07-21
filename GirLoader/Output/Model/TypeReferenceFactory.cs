using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GirLoader.Output.Model
{
    internal class TypeReferenceFactory
    {
        public ResolveableTypeReference CreateResolveable(string? name, string? ctype)
        {
            return new ResolveableTypeReference(
                symbolNameReference: GetSymbolNameReference(name),
                ctype: GetCType(ctype)
            );
        }

        public TypeReference Create(Input.Model.AnyType anyType)
        {
            if (TryCreateResolveableTypeReference(anyType, out var typeRefernece))
                return typeRefernece;

            if (TryCreateArrayTypeReference(anyType, out var arrayTypeRefernece))
                return arrayTypeRefernece;

            return CreateResolveable("void", "none");
        }

        private bool TryCreateResolveableTypeReference(Input.Model.AnyType anyType, [NotNullWhen(true)] out TypeReference? typeReference)
        {
            if (anyType.Type is null)
            {
                typeReference = null;
                return false;
            }

            typeReference = new ResolveableTypeReference(
                symbolNameReference: GetSymbolNameReference(anyType.Type.Name),
                ctype: GetCType(anyType.Type.CType));

            return true;
        }

        private bool TryCreateArrayTypeReference(Input.Model.AnyType anyType, [NotNullWhen(true)] out ArrayTypeReference? arrayTypeReference)
        {
            if (anyType.Array is null)
            {
                arrayTypeReference = null;
                return false;
            }

            var typeReference = Create(anyType.Array);

            int? length = int.TryParse(anyType.Array.Length, out var l) ? l : null;
            int? fixedSize = int.TryParse(anyType.Array.FixedSize, out var f) ? f : null;

            arrayTypeReference = new ArrayTypeReference(
                typeReference: typeReference,
                symbolNameReference: null,
                ctype: GetCType(anyType.Array.CType))
            {
                Length = length, 
                FixedSize = fixedSize, 
                IsZeroTerminated = anyType.Array.ZeroTerminated
            };
            
            return true;
        }

        public IEnumerable<TypeReference> Create(IEnumerable<Input.Model.Implement> implements)
        {
            var list = new List<TypeReference>();

            foreach (Input.Model.Implement implement in implements)
            {
                if (implement.Name is null)
                    throw new Exception("Implement is missing a name");

                list.Add(CreateResolveable(implement.Name, null));
            }

            return list;
        }

        private static SymbolNameReference? GetSymbolNameReference(string? name)
        {
            if (name is null)
                return null;

            if (!name.Contains("."))
                return new SymbolNameReference(new SymbolName(name));

            var parts = name.Split('.', 2);

            return new SymbolNameReference(
                new SymbolName(parts[1]), 
                new NamespaceName(parts[0])
            );
        }

        private static CTypeReference? GetCType(string? ctype)
        {
            if (ctype is null)
                return null;

            return new CTypeReference(ctype);
        }
    }
}
