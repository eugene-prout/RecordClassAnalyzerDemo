using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RecordClassFixer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RecordClassFixerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DNO1";
        public const string HelpUri = @"https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record";

        private const string Title = "Records should be all record classes";
        private const string MessageFormat = "Type '{0}' should be declared with the record class keywords";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpUri);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration);
        }

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is RecordDeclarationSyntax recordDecl &&
                (recordDecl.IsKind(SyntaxKind.RecordStructDeclaration) || recordDecl.ClassOrStructKeyword.IsKind(SyntaxKind.None)))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    recordDecl.ClassOrStructKeyword.IsKind(SyntaxKind.None)
                        ? recordDecl.GetLocation()
                        : recordDecl.ClassOrStructKeyword.GetLocation(),
                    recordDecl.Identifier.Text);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
