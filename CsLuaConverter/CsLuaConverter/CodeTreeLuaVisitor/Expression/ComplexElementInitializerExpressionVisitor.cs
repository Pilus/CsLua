﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Expression
{
    using CodeTree;
    using Microsoft.CodeAnalysis.CSharp;
    using Name;
    using Providers;

    public class ComplexElementInitializerExpressionVisitor : BaseVisitor
    {
        private readonly IVisitor index;
        private readonly IVisitor valueVisitor;

        public ComplexElementInitializerExpressionVisitor(CodeTreeBranch branch) : base(branch)
        {
            this.ExpectKind(0, SyntaxKind.OpenBraceToken);
            this.ExpectKind(2, SyntaxKind.CommaToken);
            this.ExpectKind(4, SyntaxKind.CloseBraceToken);
            this.index = this.CreateVisitor(1);
            this.valueVisitor = this.CreateVisitor(3);

        }

        public override void Visit(IIndentedTextWriterWrapper textWriter, IProviders providers)
        {
            textWriter.Write("[");
            this.index.Visit(textWriter, providers);
            textWriter.Write("] = ");
            this.valueVisitor.Visit(textWriter, providers);
        }
    }
}