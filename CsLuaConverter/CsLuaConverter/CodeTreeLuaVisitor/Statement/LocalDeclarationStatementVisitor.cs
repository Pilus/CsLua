﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Statement
{
    using System.CodeDom.Compiler;
    using CodeTree;
    using Microsoft.CodeAnalysis.CSharp;
    using Providers;

    public class LocalDeclarationStatementVisitor : BaseVisitor
    {
        private readonly VariableDeclarationVisitor variableDeclarationVisitor;
        public LocalDeclarationStatementVisitor(CodeTreeBranch branch) : base(branch)
        {
            this.ExpectKind(0, SyntaxKind.VariableDeclaration);
            this.ExpectKind(1, SyntaxKind.SemicolonToken);
            this.variableDeclarationVisitor = (VariableDeclarationVisitor)this.CreateVisitor(0);
        }

        public override void Visit(IndentedTextWriter textWriter, IProviders providers)
        {
            this.variableDeclarationVisitor.Visit(textWriter, providers);
            textWriter.WriteLine(";");
        }
    }
}