namespace CsLuaConverter.CodeTreeLuaVisitor.Expression.Binary
{
    using CodeTree;
    using Microsoft.CodeAnalysis.CSharp;

    public class LogicalOrExpressionVisitor : BinaryExpressionVisitorBase
    {
        public LogicalOrExpressionVisitor(CodeTreeBranch branch) : base(branch, SyntaxKind.BarBarToken, "or")
        {
        }
    }
}