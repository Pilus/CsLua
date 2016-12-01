﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Member
{
    using System.Linq;
    using CodeTree;
    using CsLuaConverter.Context;
    using CsLuaConverter.SyntaxExtensions;
    using Lists;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ThisConstructorInitializerVisitor : BaseVisitor
    {
        public ThisConstructorInitializerVisitor(CodeTreeBranch branch) : base(branch)
        {
        }

        public override void Visit(IIndentedTextWriterWrapper textWriter, IContext context)
        {
            var syntax = (ConstructorInitializerSyntax)this.Branch.SyntaxNode;
            Write(syntax, textWriter, context);
        }

        public static void Write(ConstructorInitializerSyntax syntax, IIndentedTextWriterWrapper textWriter, IContext context)
        {
            var symbol = (IMethodSymbol)context.SemanticModel.GetSymbolInfo(syntax).Symbol;

            textWriter.Write("(element % _M.DOT_LVL(typeObject.Level))");

            var signatureWriter = textWriter.CreateTextWriterAtSameIndent();
            var hasGenericComponents = context.SignatureWriter.WriteSignature(symbol.Parameters.Select(p => p.Type).ToArray(), signatureWriter);

            if (hasGenericComponents)
            {
                textWriter.Write("['_C_0_'..(");
                textWriter.AppendTextWriter(signatureWriter);
                textWriter.Write(")]");
            }
            else
            {
                textWriter.Write("._C_0_");
                textWriter.AppendTextWriter(signatureWriter);
            }

            syntax.ArgumentList.Write(textWriter, context);

            textWriter.WriteLine(";");
        }
    }
}