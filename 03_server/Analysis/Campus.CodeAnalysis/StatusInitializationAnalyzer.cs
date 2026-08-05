using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Campus.CodeAnalysis;

/// <summary>
/// 校园平台代码规范分析器。
/// CAMPUS001: [FromBody] 接收的实体若含 Status 属性，必须在方法体内显式赋值为 1。
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StatusInitializationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CAMPUS001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "实体 Status 属性必须初始化为 1",
        messageFormat: "实体 '{0}' 含有 Status 属性，但方法体中未将 '{1}.Status' 显式设为 1。未初始化会导致数据不可见。",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "通过 [FromBody] 接收的实体如果含有 int Status 属性，必须在方法体中显式设置 Status = 1，"
                     + "否则客户端可能传入 0 导致数据被过滤查询排除（如评论、帖子、商品等不可见）。");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        // 0. 只检查 [HttpPost] 方法（创建操作），跳过 [HttpPut]（更新操作）
        if (!HasAttribute(method, "HttpPost"))
            return;

        // 1. 查找 [FromBody] 参数
        var fromBodyParam = FindFromBodyParameter(method);
        if (fromBodyParam == null) return;

        // 2. 通过语义模型获取参数类型
        var paramType = context.SemanticModel.GetTypeInfo(fromBodyParam.Type!).Type;
        if (paramType == null) return;

        // 3. 检查参数类型是否有 int Status 属性
        //    跳过 DTO 类型（Request/Dto/ViewModel 后缀），只检查实体类型
        if (IsDtoType(paramType.Name))
            return;

        var statusProp = paramType.GetMembers("Status")
            .OfType<IPropertySymbol>()
            .FirstOrDefault(p => p.Type.SpecialType == SpecialType.System_Int32);
        if (statusProp == null) return;

        // 4. 检查方法体是否包含 paramName.Status = 1
        var paramName = fromBodyParam.Identifier.ValueText;
        if (method.Body == null) return;

        if (HasStatusInitialization(method.Body, paramName))
            return; // 已正确初始化

        // 5. 报告诊断
        var diagnostic = Diagnostic.Create(Rule, fromBodyParam.GetLocation(), paramType.Name, paramName);
        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// 判断类型名称是否为 DTO（而非实体），跳过这些类型的检查。
    /// </summary>
    private static bool IsDtoType(string typeName)
    {
        return typeName.EndsWith("Request") ||
               typeName.EndsWith("Dto") ||
               typeName.EndsWith("ViewModel") ||
               typeName.EndsWith("Model");
    }

    /// <summary>
    /// 检查方法是否带有指定名称的特性（如 HttpPost、HttpPut）。
    /// </summary>
    private static bool HasAttribute(MethodDeclarationSyntax method, string attributeName)
    {
        foreach (var attrList in method.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                if (attr.Name.ToString().Contains(attributeName))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 在方法参数列表中查找带 [FromBody] 特性的参数。
    /// </summary>
    private static ParameterSyntax? FindFromBodyParameter(MethodDeclarationSyntax method)
    {
        foreach (var param in method.ParameterList.Parameters)
        {
            foreach (var attrList in param.AttributeLists)
            {
                foreach (var attr in attrList.Attributes)
                {
                    var name = attr.Name.ToString();
                    if (name.Contains("FromBody"))
                        return param;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 使用语法树遍历检查方法体是否包含 paramName.Status = 1 赋值。
    /// 比 regex 更精准，不会误匹配字符串或注释中的内容。
    /// </summary>
    private static bool HasStatusInitialization(BlockSyntax body, string paramName)
    {
        foreach (var assignment in body.DescendantNodes()
            .OfType<AssignmentExpressionSyntax>())
        {
            // 检查左侧是否为 paramName.Status
            if (assignment.Left is MemberAccessExpressionSyntax memberAccess)
            {
                // 提取接收者标识符
                if (memberAccess.Expression is IdentifierNameSyntax identifier &&
                    identifier.Identifier.ValueText == paramName &&
                    memberAccess.Name.Identifier.ValueText == "Status")
                {
                    // 检查右侧是否为字面量 1
                    if (assignment.Right is LiteralExpressionSyntax literal &&
                        literal.Token.ValueText == "1")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
