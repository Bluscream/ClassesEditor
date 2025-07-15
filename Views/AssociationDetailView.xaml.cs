using System.Windows;
using System.Windows.Controls;
using ClassesEditor.ViewModels;

namespace Views
{
    public partial class AssociationDetailView : UserControl
    {
        public AssociationDetailView()
        {
            InitializeComponent();
        }

        private void OpenInRegedit_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssociationDetailViewModel vm)
            {
                vm.OpenInRegistryEditor();
            }
        }
    }
} 