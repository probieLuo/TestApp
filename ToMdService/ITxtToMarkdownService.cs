namespace ToMdService
{
    /// <summary>
    /// TXT转Markdown服务接口
    /// </summary>
    public interface ITxtToMarkdownService
    {
        /// <summary>
        /// 将TXT文件转换为Markdown
        /// </summary>
        /// <param name="txtFilePath">TXT文件路径</param>
        /// <param name="mdFilePath">输出的Markdown文件路径</param>
        /// <returns>转换是否成功</returns>
        Task<bool> ConvertToMarkdownAsync(string txtFilePath, string mdFilePath);

        /// <summary>
        /// 自动生成Markdown文件路径
        /// </summary>
        /// <param name="txtFilePath">TXT文件路径</param>
        /// <returns>对应的Markdown文件路径</returns>
        string GenerateMdFilePath(string txtFilePath);

        /// <summary>
        /// 转换进度事件
        /// </summary>
        event EventHandler<ConversionProgressEventArgs> ConversionProgressChanged;

        /// <summary>
        /// 转换完成事件
        /// </summary>
        event EventHandler<ConversionCompletedEventArgs> ConversionCompleted;
    }

    /// <summary>
    /// 转换进度事件参数
    /// </summary>
    public class ConversionProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 进度百分比
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// 当前状态信息
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// 转换完成事件参数
    /// </summary>
    public class ConversionCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string OutputFilePath { get; set; }

        /// <summary>
        /// 错误信息（如果失败）
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}