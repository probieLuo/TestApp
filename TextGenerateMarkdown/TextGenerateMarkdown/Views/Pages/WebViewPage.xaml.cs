using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestApp.ViewModels.Pages;

namespace TestApp.Views.Pages
{
	/// <summary>
	/// WebViewPage.xaml 的交互逻辑
	/// </summary>
	public partial class WebViewPage : Page
	{
        private readonly WebViewModel _viewModel;

        public WebViewPage(WebViewModel viewModel)
		{

			InitializeComponent();

            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.Initialize(webView);
        }

        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        => _viewModel.OnNavigationCompleted(sender, e);
    }
}
