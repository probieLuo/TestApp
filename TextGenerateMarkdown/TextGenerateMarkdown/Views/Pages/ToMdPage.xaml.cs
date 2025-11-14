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
using Wpf.Ui.Abstractions.Controls;

namespace TestApp.Views.Pages
{
	/// <summary>
	/// ToMdPage.xaml 的交互逻辑
	/// </summary>
	public partial class ToMdPage :  INavigableView<ToMdViewModel>
	{
        public ToMdViewModel ViewModel { get; }

        public ToMdPage(ToMdViewModel viewModel)
		{
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            // 监听ViewModel的属性变化
            ViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ViewModel.CurrentMdStr))
                {
                    PreviewBrowser.Visibility = Visibility.Visible;
                    PreviewBrowser.NavigateToString(ViewModel.CurrentMdStr);
                }
            };
        }
	}
}
