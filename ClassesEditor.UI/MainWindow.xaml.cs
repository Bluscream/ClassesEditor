using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassesEditor.ViewModels;

namespace ClassesEditor.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel _fileTypeViewModel = new MainViewModel(AssociationFilterMode.FileType);
    private MainViewModel _protocolViewModel = new MainViewModel(AssociationFilterMode.Protocol);

    public MainWindow()
    {
        InitializeComponent();
        // Set DataContext for each tab's Grid after loading
        this.Loaded += (s, e) =>
        {
            var tabControl = (TabControl)this.Content;
            var fileTypeTab = (TabItem)tabControl.Items[0];
            var fileTypeGrid = (Grid)fileTypeTab.Content;
            fileTypeGrid.DataContext = _fileTypeViewModel;

            var protocolTab = (TabItem)tabControl.Items[1];
            var protocolGrid = (Grid)protocolTab.Content;
            protocolGrid.DataContext = _protocolViewModel;
        };
    }

    private void Refresh_Click_FileType(object sender, RoutedEventArgs e)
    {
        _fileTypeViewModel.RefreshAssociations();
    }

    private void Refresh_Click_Protocol(object sender, RoutedEventArgs e)
    {
        _protocolViewModel.RefreshAssociations();
    }
}