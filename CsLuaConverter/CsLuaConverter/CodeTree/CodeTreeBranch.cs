﻿namespace CsLuaConverter.CodeTree
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    [DebuggerDisplay("CodeTreeBranch - {Kind}")]
    public class CodeTreeBranch : CodeTreeNode
    {
        public CodeTreeNode[] Nodes;
        public string DocumentName;

        public CodeTreeBranch(SyntaxNode node, string documentName)
        {
            this.Kind = node.GetKind();
            this.Nodes = this.GetNodes(node);
            this.DocumentName = documentName;
        }

        public CodeTreeBranch(SyntaxNode node) : this(node, null)
        {
            
        }

        private CodeTreeNode[] GetNodes(SyntaxNode node)
        {
            var nodes = new List<CodeTreeNode>();
            var token = node.GetFirstToken();
            var lastToken = node.GetLastToken();

            while (token != lastToken.GetNextToken())
            {
                if (token.Parent == node)
                {
                    nodes.Add(new CodeTreeLeaf(token));
                }
                else
                {
                    var subNode = token.GetChildOfAnchestor(node);
                    nodes.Add(new CodeTreeBranch(subNode));
                    token = subNode.GetLastToken();
                }

                token = token.GetNextToken();
            }

            return nodes.ToArray();
        }
    }
}