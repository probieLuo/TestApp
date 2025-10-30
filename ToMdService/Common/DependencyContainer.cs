using Microsoft.Extensions.DependencyInjection;

namespace ToMdService.Common
{
    /// <summary>
    /// 依赖注入容器
    /// </summary>
    public static class DependencyContainer
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        public static IServiceCollection AddTxtToMarkdownServices(this IServiceCollection services)
        {
            // 注册服务
            services.AddSingleton<ITxtToMarkdownService, TxtToMarkdownService>();
            
            return services;
        }

        /// <summary>
        /// 创建服务提供器
        /// </summary>
        public static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddTxtToMarkdownServices();
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// 获取ITxtToMarkdownService实例
        /// </summary>
        public static ITxtToMarkdownService GetTxtToMarkdownService()
        {
            var serviceProvider = CreateServiceProvider();
            return serviceProvider.GetRequiredService<ITxtToMarkdownService>();
        }
    }
}