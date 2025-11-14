# TestApp

简要说明

TestApp 是一个包含多个子项目的 WPF 桌面应用示例集合，主要包含：

- `TextGenerateMarkdown`：与文本/网文转换为 Markdown 相关的前端 WPF 应用 UI 与视图模型。
- `MarkdownService`：用于生成/处理 Markdown 的服务库。
- `NotificationThrottle`：示例通知节流服务与相关 UI 示例。

仓库目标

- 演示如何在 WPF (.NET 9) 应用中组织多个项目与服务。
- 提供文本处理与 Markdown 生成的示例实现。
- 展示依赖注入、MVVM 与托管主机构建（Generic Host）的用法。

先决条件

- .NET 9 SDK
- Windows（运行 WPF 桌面应用）
- 推荐使用 Visual Studio 2022/2026 或 JetBrains Rider 支持 WPF 开发

快速开始

1. 克隆仓库：

   git clone <仓库地址>

2. 在仓库根目录打开终端，恢复并构建：

   dotnet restore
   
   dotnet build

4. 运行 WPF 应用（示例）：

   dotnet run --project NotificationThrottle/NotificationThrottle

项目结构（概要）

- `TextGenerateMarkdown/` - Txt转Markdown 视图
- `MarkdownService/` - Markdown 生成与处理库
- `NotificationThrottle/` - 通知节流服务示例

贡献

欢迎提交 Issue 与 Pull Request。

联系

如需帮助，请在仓库中打开 Issue。
