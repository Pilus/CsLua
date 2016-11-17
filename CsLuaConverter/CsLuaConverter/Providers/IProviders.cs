namespace CsLuaConverter.Providers
{
    using CsLuaConverter.CodeTreeLuaVisitor;
    using CsLuaConverter.MethodSignature;
    using Microsoft.CodeAnalysis;

    public interface IProviders
    {
        PartialElementState PartialElementState { get; }
        SignatureWriter<ITypeSymbol> SignatureWriter { get; }
        ITypeReferenceWriter<ITypeSymbol> TypeReferenceWriter { get; }
        ISemanticAdaptor<ITypeSymbol> SemanticAdaptor { get; }
        SemanticModel SemanticModel { get; set; }
        INamedTypeSymbol CurrentClass { get; set; }
    }
}