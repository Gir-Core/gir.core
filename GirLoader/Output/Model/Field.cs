using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace GirLoader.Output.Model
{
    public class Field : Symbol, AnyType
    {
        public TypeReference TypeReference { get; }

        public Callback? Callback { get; }
        public bool Readable { get; }
        public bool Private { get; }

        /// <summary>
        /// Creates a new field.
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="typeReference"></param>
        /// <param name="typeInformation"></param>
        /// <param name="callback">Optional: If set it is expected that the callback belongs to the given symbol reference.</param>
        /// <param name="readable"></param>
        /// <param name="private"></param>
        /// <param name="orignalName"></param>
        public Field(SymbolName orignalName, SymbolName symbolName, TypeReference typeReference, bool readable = true, bool @private = false) : base(orignalName, symbolName)
        {
            TypeReference = typeReference;
            Readable = readable;
            Private = @private;
        }

        public Field(SymbolName orignalName, SymbolName symbolName, ResolveableTypeReference resolveableTypeReference, Callback callback, bool readable = true, bool @private = false) 
            : this(orignalName, symbolName, resolveableTypeReference, readable, @private)
        {
            Callback = callback;
            resolveableTypeReference.ResolveAs(Callback);
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
        {
            // If the callback is not null this means the symbol reference
            // of this field was already resolved automatically. Meaning we
            // do not return the symbol reference to the caller but the
            // symbol references of the callback instead.
            //
            // If the callback is null the symbol reference is still unresolved
            // and must be returned to get resolved.

            if (Callback is not null)
                return Callback.GetTypeReferences();
            else
                return new List<TypeReference>() { TypeReference };
        }

        public override bool GetIsResolved()
            => TypeReference.GetIsResolved() && (Callback?.GetIsResolved() ?? true);
    }
}
