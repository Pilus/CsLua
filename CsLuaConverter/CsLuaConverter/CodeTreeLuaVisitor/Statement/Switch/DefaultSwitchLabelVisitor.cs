﻿namespace CsLuaConverter.CodeTreeLuaVisitor.Statement.Switch
{
    using CodeTree;
    using CsLuaConverter.Context;

    public class DefaultSwitchLabelVisitor : BaseVisitor, ISwitchLabelVisitor
    {
        public DefaultSwitchLabelVisitor(CodeTreeBranch branch) : base(branch)
        {
        }

        public override void Visit(IIndentedTextWriterWrapper textWriter, IContext context)
        {
            textWriter.Write("true");
        }
    }
}