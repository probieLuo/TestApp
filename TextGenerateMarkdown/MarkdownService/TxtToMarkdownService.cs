using System.Text;
using System.Text.RegularExpressions;

namespace ToMdService
{
    /// <summary>
    /// TXT转Markdown服务实现
    /// </summary>
    public class TxtToMarkdownService : ITxtToMarkdownService
    {
        /// <summary>
        /// 转换进度事件
        /// </summary>
        public event EventHandler<ConversionProgressEventArgs> ConversionProgressChanged;

        /// <summary>
        /// 转换完成事件
        /// </summary>
        public event EventHandler<ConversionCompletedEventArgs> ConversionCompleted;

        /// <summary>
        /// 将TXT文件转换为Markdown
        /// </summary>
        /// <param name="txtFilePath">TXT文件路径</param>
        /// <param name="mdFilePath">输出的Markdown文件路径</param>
        /// <returns>转换是否成功</returns>
        public async Task<bool> ConvertToMarkdownAsync(string txtFilePath, string mdFilePath)
        {
            try
            {
                OnConversionProgressChanged(0, "开始转换...");

                // 验证文件是否存在
                if (!File.Exists(txtFilePath))
                {
                    throw new FileNotFoundException("TXT文件不存在", txtFilePath);
                }

                OnConversionProgressChanged(10, "读取文件内容...");

                // 读取TXT文件，自动检测编码
                string content = await ReadFileWithAutoEncodingAsync(txtFilePath);

                OnConversionProgressChanged(30, "分析文本结构...");

                // 转换为Markdown
                string markdownContent = await Task.Run(() => ConvertToMarkdown(content));

                OnConversionProgressChanged(70, "保存Markdown文件...");

                // 保存Markdown文件
                await SaveMarkdownFileAsync(mdFilePath, markdownContent);

                OnConversionProgressChanged(100, "转换完成！");

                // 触发转换完成事件
                OnConversionCompleted(true, mdFilePath, null);

                return true;
            }
            catch (Exception ex)
            {
                OnConversionProgressChanged(0, $"转换失败: {ex.Message}");
                OnConversionCompleted(false, null, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 自动生成Markdown文件路径
        /// </summary>
        /// <param name="txtFilePath">TXT文件路径</param>
        /// <returns>对应的Markdown文件路径</returns>
        public string GenerateMdFilePath(string txtFilePath)
        {
            string directory = Path.GetDirectoryName(txtFilePath);
            string fileName = Path.GetFileNameWithoutExtension(txtFilePath);
            return Path.Combine(directory, $"{fileName}.md");
        }

        /// <summary>
        /// 异步读取文件，自动检测编码
        /// </summary>
        private async Task<string> ReadFileWithAutoEncodingAsync(string filePath)
        {
            //// 尝试常见编码
            //Encoding[] encodings = {
            //    Encoding.UTF8,
            //    Encoding.Default,
            //    Encoding.GetEncoding("GB2312"),
            //    Encoding.GetEncoding("GBK"),
            //    Encoding.Unicode,
            //    Encoding.BigEndianUnicode
            //};

            //foreach (Encoding encoding in encodings)
            //{
            //    try
            //    {
            //        return await File.ReadAllTextAsync(filePath, encoding);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}
            
            // 如果都失败，使用默认编码
            return await File.ReadAllTextAsync(filePath, Encoding.GetEncoding("GBK"));
        }

        /// <summary>
        /// 将文本转换为Markdown格式
        /// </summary>
        private string ConvertToMarkdown(string content)
        {
            List<string> lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            List<string> markdownLines = new List<string>();

            bool inParagraph = false;
            int paragraphLineCount = 0;
            List<string> currentParagraph = new List<string>();

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string trimmedLine = line.Trim();

                // 更新进度
                int progress = (int)((i + 1) * 40.0 / lines.Count) + 30; // 30-70% 是分析阶段
                if (progress > 70) progress = 70;
                OnConversionProgressChanged(progress, $"分析第 {i + 1}/{lines.Count} 行...");

                // 空行处理
                if (string.IsNullOrWhiteSpace(line))
                {
                    markdownLines.Add(""); // 保持空行
                    continue;
                }

                // 处理非空行
                if (IsTitleLine(trimmedLine))
                {
                    // 处理标题
                    string titleLine = ProcessTitleLine(trimmedLine);
                    markdownLines.Add(titleLine);
                }
                else
                {
                    // 普通文本，加入段落
                    markdownLines.Add("&emsp;  "+line);
                    markdownLines.Add("");
                }
            }

            // 清理多余的空行
            return CleanupMarkdown(string.Join(Environment.NewLine, markdownLines));
        }

        /// <summary>
        /// 异步保存Markdown文件
        /// </summary>
        private async Task SaveMarkdownFileAsync(string filePath, string content)
        {
            // 创建目录（如果不存在）
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// 检测是否为标题行
        /// </summary>
        private bool IsTitleLine(string line)
        {
            // 检测标题行的模式
            return Regex.IsMatch(line, @"^第[一二三四五六七八九十\d]+[章节回卷集篇部]", RegexOptions.Compiled) ||
                   Regex.IsMatch(line, @"^第[一二三四五六七八九十\d]+[章节回卷集篇部]$", RegexOptions.Compiled) ||
                   Regex.IsMatch(line, @"^[卷篇部集][一二三四五六七八九十\d]+[：:]", RegexOptions.Compiled) ||
                   Regex.IsMatch(line, @"^[=*#-—]{3,}.*[=*#-—]{3,}$", RegexOptions.Compiled) ||
                   Regex.IsMatch(line, @"^[\d]+[.、)]\s*[\u4e00-\u9fa5]+", RegexOptions.Compiled);
        }

        /// <summary>
        /// 处理标题行
        /// </summary>
        private string ProcessTitleLine(string line)
        {
            string trimmedLine = line.Trim();

            if (Regex.IsMatch(trimmedLine, @"^第[一二三四五六七八九十\d]+[卷部集]", RegexOptions.Compiled))
            {
                return $"# {trimmedLine}";
            }
            else if (Regex.IsMatch(trimmedLine, @"^第[一二三四五六七八九十\d]+[章节回篇]", RegexOptions.Compiled))
            {
                return $"## {trimmedLine}";
            }
            else if (Regex.IsMatch(trimmedLine, @"^[\d]+[.、)]\s*[\u4e00-\u9fa5]+", RegexOptions.Compiled))
            {
                return $"### {trimmedLine}";
            }
            else if (Regex.IsMatch(trimmedLine, @"^[=*#-—]{3,}.*[=*#-—]{3,}$", RegexOptions.Compiled))
            {
                string cleanTitle = Regex.Replace(trimmedLine, @"^[=*#-—]+|[=*#-—]+$", "").Trim();
                return $"## {cleanTitle}";
            }

            return $"## {trimmedLine}";
        }

        /// <summary>
        /// 检测是否为引用行
        /// </summary>
        private bool IsQuoteLine(string line)
        {
            string trimmedLine = line.TrimStart();
            return trimmedLine.StartsWith("\"") || trimmedLine.StartsWith("“") ||
                   trimmedLine.StartsWith("『") || trimmedLine.StartsWith("「");
        }

        /// <summary>
        /// 处理引用行
        /// </summary>
        private string ProcessQuoteLine(string line)
        {
            string trimmedLine = line.TrimStart();

            if (trimmedLine.StartsWith("\"") || trimmedLine.StartsWith("“") ||
                trimmedLine.StartsWith("『") || trimmedLine.StartsWith("「"))
            {
                trimmedLine = trimmedLine.Substring(1).TrimStart();
            }

            if (trimmedLine.EndsWith("\"") || trimmedLine.EndsWith("”") ||
                trimmedLine.EndsWith("』") || trimmedLine.EndsWith("」"))
            {
                trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 1).TrimEnd();
            }

            return $"> {trimmedLine}";
        }

        /// <summary>
        /// 清理Markdown格式
        /// </summary>
        private string CleanupMarkdown(string markdown)
        {
            markdown = Regex.Replace(markdown, @"\n{3,}", "\n\n", RegexOptions.Compiled);
            markdown = Regex.Replace(markdown, @"^[ \t]+", "", RegexOptions.Compiled | RegexOptions.Multiline);
            markdown = Regex.Replace(markdown, @"^(#+ .+)$", "$1\n", RegexOptions.Compiled | RegexOptions.Multiline);

            return markdown.Trim() + "\n";
        }

        /// <summary>
        /// 触发进度更新事件
        /// </summary>
        protected virtual void OnConversionProgressChanged(int progressPercentage, string status)
        {
            ConversionProgressChanged?.Invoke(this, new ConversionProgressEventArgs
            {
                ProgressPercentage = progressPercentage,
                Status = status
            });
        }

        /// <summary>
        /// 触发转换完成事件
        /// </summary>
        protected virtual void OnConversionCompleted(bool success, string outputFilePath, string errorMessage)
        {
            ConversionCompleted?.Invoke(this, new ConversionCompletedEventArgs
            {
                Success = success,
                OutputFilePath = outputFilePath,
                ErrorMessage = errorMessage
            });
        }
    }
}