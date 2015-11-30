﻿namespace CsLuaConverter.CodeElementAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public abstract class BaseElement
    {
        public static void ExpectKind(SyntaxKind expectedKind, SyntaxKind actualKind)
        {
            if (!expectedKind.Equals(actualKind))
            {
                throw new Exception($"Unexpected token. Expected {expectedKind}, got {actualKind}.");
            }
        }

        private static readonly Dictionary<SyntaxKind, Func<BaseElement>> NodeMappings = new Dictionary
            <SyntaxKind, Func<BaseElement>>()
        {
            { SyntaxKind.NamespaceDeclaration,          () => new NamespaceDeclaration() },
            { SyntaxKind.ClassDeclaration,              () => new ClassDeclaration() },
            { SyntaxKind.IdentifierName,                () => new IdentifierName() },
            { SyntaxKind.ConstructorDeclaration,        () => new ConstructorDeclaration() },
            { SyntaxKind.Block,                         () => new Block() },
            { SyntaxKind.SimpleAssignmentExpression,    () => new SimpleAssignmentExpression() },
            { SyntaxKind.StringLiteralExpression,       () => new StringLiteralExpression() },
            { SyntaxKind.ThisExpression,                () => new ThisExpression() },
            { SyntaxKind.BracketedArgumentList,         () => new BracketedArgumentList() },
            { SyntaxKind.MethodDeclaration,             () => new MethodDeclaration() },
            { SyntaxKind.PredefinedType,                () => new PredefinedType() },
            { SyntaxKind.VariableDeclarator,            () => new VariableDeclarator() },
            { SyntaxKind.EqualsValueClause,             () => new EqualsValueClause() },
            { SyntaxKind.ObjectCreationExpression,      () => new ObjectCreationExpression() },
            { SyntaxKind.ArgumentList,                  () => new ArgumentList() },
            { SyntaxKind.SimpleMemberAccessExpression,  () => new SimpleMemberAccessExpression() },
            { SyntaxKind.NumericLiteralExpression,      () => new NumericLiteralExpression() },
            { SyntaxKind.TrueLiteralExpression,         () => new TrueLiteralExpression() },
            { SyntaxKind.NullLiteralExpression,         () => new NullLiteralExpression() },
            { SyntaxKind.Parameter,                     () => new Parameter() },
            { SyntaxKind.FieldDeclaration,              () => new FieldDeclaration() },
            { SyntaxKind.GenericName,                   () => new GenericName() },
            { SyntaxKind.UsingDirective,                () => new UsingDirective() },
            { SyntaxKind.TypeParameter,                 () => new TypeParameter() },
        };

        public abstract SyntaxToken Analyze(SyntaxToken token);

        public static BaseElement GenerateMatchingElement(SyntaxToken token)
        {
            var mapping = NodeMappings.FirstOrDefault(m => token.Parent.IsKind(m.Key)).Value;
            if (mapping == null)
            {
                throw new Exception($"Could not find a mapping for parent syntax kind. {token.Parent.GetKind()}");
            }

            return mapping();
        }
    }
}