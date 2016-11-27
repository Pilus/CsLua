﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Name
{
    using System.Linq;
    using CodeTree;
    using CsLuaConverter.Context;
    using Filters;
    using Microsoft.CodeAnalysis.CSharp;

    public class QualifiedNameVisitor : BaseVisitor, INameVisitor
    {
        private readonly INameVisitor[] visitors;

        public QualifiedNameVisitor(CodeTreeBranch branch) : base(branch)
        {
            this.visitors = this.CreateVisitors(new KindFilter(SyntaxKind.IdentifierName, SyntaxKind.QualifiedName, SyntaxKind.GenericName))
                .OfType<INameVisitor>().ToArray();
        }

        public override void Visit(IIndentedTextWriterWrapper textWriter, IContext context)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetName()
        {
            return this.visitors.SelectMany(v => v.GetName()).ToArray();
        }
    }
}