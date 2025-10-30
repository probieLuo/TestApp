using Markdig;
using Microsoft.Win32;
using ToMdService;
using Wpf.Ui.Abstractions.Controls;

namespace TestApp.ViewModels.Pages
{
    public partial class ToMdViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private ToMdService.ITxtToMarkdownService _toMdService;

        public ToMdViewModel(ITxtToMarkdownService txtToMarkdown)
        {
            _toMdService = txtToMarkdown;
        }

        private void InitializeViewModel()
        {
            // Initialization logic here
            //_toMdService = ToMdService.Common.DependencyContainer.GetTxtToMarkdownService();

            // 订阅服务事件
            _toMdService.ConversionProgressChanged += OnConversionProgressChanged;
            _toMdService.ConversionCompleted += OnConversionCompleted;

            _isInitialized = true;
        }

        [ObservableProperty]
        private string _txtPath = String.Empty;

        [RelayCommand]
        private void OnSelectTxtPath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "文本文件 (*.txt)|*.txt",
                Title = "选择 txt 文件"
            };

            if (dialog.ShowDialog() == true)
            {
                TxtPath = dialog.FileName;
            }
        }

        [RelayCommand]
        private async Task OnConvertToMd()
        {
            if (string.IsNullOrEmpty(TxtPath))
            {
                StatusMessage = "请先选择TXT文件";
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = "开始转换...";
                ProgressValue = 0;

                string mdFilePath = _toMdService.GenerateMdFilePath(TxtPath);
                await _toMdService.ConvertToMarkdownAsync(TxtPath, mdFilePath);
            }
            catch (Exception ex)
            {
                StatusMessage = $"转换失败: {ex.Message}";
                IsBusy = false;
            }
        }

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private int _progressValue = 0;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        /// <summary>
        /// 处理转换进度事件
        /// </summary>
        private void OnConversionProgressChanged(object sender, ConversionProgressEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
            StatusMessage = e.Status;
        }

        /// <summary>
        /// 处理转换完成事件
        /// </summary>
        private void OnConversionCompleted(object sender, ConversionCompletedEventArgs e)
        {
            IsBusy = false;
            ProgressValue = 0;

            if (e.Success)
            {
                StatusMessage = $"转换成功！文件已保存为: {System.IO.Path.GetFileName(e.OutputFilePath)}";

                // 读取文件内容
                var fullContent = System.IO.File.ReadAllText(e.OutputFilePath);

                // 截取前1000字符（处理短文件）
                var previewContent = fullContent.Length > 10000
                    ? fullContent.Substring(0, 10000) + "..."
                    : fullContent;

                // 转换为HTML并添加样式
                var htmlContent = $@"
                <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{ font-family: 'Segoe UI', sans-serif; line-height: 1.6; padding: 20px; }}
                            h1, h2, h3 {{ color: #333; }}
                            code {{ background: #f5f5f5; padding: 2px 4px; border-radius: 3px; }}
                            pre {{ background: #f5f5f5; padding: 10px; border-radius: 5px; overflow: auto; }}
                        </style>
                    </head>
                    <body>
                        {Markdown.ToHtml(previewContent)}
                    </body>
                </html>";

                // 更新CurrentFile（自动通知UI刷新）
                CurrentMdStr = htmlContent;
            }
            else
            {
                StatusMessage = $"转换失败: {e.ErrorMessage}";
            }
        }

        [ObservableProperty]
        private string _currentMdStr = string.Empty;
    }
}
