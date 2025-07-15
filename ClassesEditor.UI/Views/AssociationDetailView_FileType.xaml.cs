using System.Windows;
using System.Windows.Controls;
using ClassesEditor.ViewModels;

namespace ClassesEditor.UI.Views
{
    public partial class AssociationDetailView_FileType : UserControl
    {
        public AssociationDetailView_FileType()
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

        private void AddOpenWith_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssociationDetailViewModel vm && !string.IsNullOrWhiteSpace(OpenWithExeBox.Text))
            {
                vm.AddOpenWith(OpenWithExeBox.Text.Trim());
                OpenWithExeBox.Text = string.Empty;
            }
        }

        private void RemoveOpenWith_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssociationDetailViewModel vm && OpenWithListBox.SelectedItem is string exe)
            {
                vm.RemoveOpenWith(exe);
            }
        }

        private void AddOpenWithProgid_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssociationDetailViewModel vm && !string.IsNullOrWhiteSpace(OpenWithProgidBox.Text))
            {
                vm.AddOpenWithProgid(OpenWithProgidBox.Text.Trim());
                OpenWithProgidBox.Text = string.Empty;
            }
        }

        private void RemoveOpenWithProgid_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssociationDetailViewModel vm && OpenWithProgidListBox.SelectedItem is string progid)
            {
                vm.RemoveOpenWithProgid(progid);
            }
        }
    }
} 