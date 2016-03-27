﻿namespace CsLuaConverter.CodeTreeLuaVisitor
{
    using System.CodeDom.Compiler;
    using System.Linq;
    using CodeTree;
    using Microsoft.CodeAnalysis.CSharp;
    using Name;
    using Providers;
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

        public override void Visit(IndentedTextWriter textWriter, IProviders providers)
        {
            throw new System.NotImplementedException();
        }

        public string GetName()
        {
            return this.name;
        }

        public void WriteDefaultValue(IndentedTextWriter textWriter, IProviders providers, ITypeVisitor typeVisitor)
        {
            textWriter.Write(this.name);

            if (this.valueVisitor != null)
            {
                this.valueVisitor.Visit(textWriter, providers);
                textWriter.WriteLine(",");
            }
            else
            {
                textWriter.Write(" = _M.DV(");
                typeVisitor.WriteAsType(textWriter, providers);
                textWriter.WriteLine("),");
            }
        }

        public void WriteInitializeValue(IndentedTextWriter textWriter, IProviders providers)
        {
            textWriter.WriteLine($"if not(values.{this.name} == nil) then element[typeObject.Level].{this.name} = values.{this.name}; end");
        }
    }
}