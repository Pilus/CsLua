﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Member
{
    using System.Linq;
    using CodeTree;
    using CsLuaConverter.Context;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Type;

    public class VariableDeclaratorVisitor : BaseVisitor
    {
        private readonly string name;
        private readonly BaseVisitor valueVisitor;

        public VariableDeclaratorVisitor(CodeTreeBranch branch) : base(branch)
        {
            this.ExpectKind(0, SyntaxKind.IdentifierToken);
            this.name = ((CodeTreeLeaf) this.Branch.Nodes.First()).Text;

            if (this.Branch.Nodes.Length > 1)
            {
                this.ExpectKind(1, SyntaxKind.EqualsValueClause);
                this.valueVisitor = this.CreateVisitor(1);
            }
        }

        public override void Visit(IIndentedTextWriterWrapper textWriter, IContext context)
        {
            textWriter.Write(this.name);
            this.valueVisitor?.Visit(textWriter, context);
        }

        public string GetName()
        {
            return this.name;
        }

        public static void WriteDefaultValue(VariableDeclaratorSyntax syntax, IIndentedTextWriterWrapper textWriter, IContext context)
        {
            textWriter.Write(syntax.Identifier.Text);

            if (syntax.Initializer != null)
            {
                EqualsValueClauseVisitor.Visit(syntax.Initializer, textWriter, context);
                textWriter.WriteLine(",");
            }
            else
            {
                var symbol = (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(syntax);
                textWriter.Write(" = _M.DV(");
                context.TypeReferenceWriter.WriteTypeReference(symbol.Type, textWriter);
                textWriter.WriteLine("),");
            }
        }
    }
}