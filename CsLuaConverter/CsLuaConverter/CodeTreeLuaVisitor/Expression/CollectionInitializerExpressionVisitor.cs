﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Expression
{
    using System.Linq;
    using CodeTree;
    using CsLuaConverter.Context;
    using Filters;
    using Microsoft.CodeAnalysis.CSharp;

    public class CollectionInitializerExpressionVisitor : BaseVisitor
    {
        private readonly BaseVisitor[] innerVisitors;
        public CollectionInitializerExpressionVisitor(CodeTreeBranch branch) : base(branch)
        {
            this.innerVisitors =
                this.CreateVisitors(new KindRangeFilter(SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken,
                    SyntaxKind.CommaToken)).ToArray();
        }

        public override void Visit(IIndentedTextWriterWrapper textWriter, IContext context)
        {
            textWriter.WriteLine(".__Initialize({");
            textWriter.Indent++;

            this.innerVisitors.VisitAll(textWriter, context, () =>
            {
                textWriter.WriteLine(",");
            });
            textWriter.Indent--;
            textWriter.WriteLine();
            textWriter.Write("})");
        }
    }
}