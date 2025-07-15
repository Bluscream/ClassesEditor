using System.Windows;
using ClassesEditor.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(AssociationFilterMode.FileType);
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.RefreshAssociations();
        }
    }
} 