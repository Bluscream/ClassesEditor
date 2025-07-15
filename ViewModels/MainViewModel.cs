using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassesEditor.Services;

namespace ClassesEditor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly RegistryService _registryService = new RegistryService();

        public ObservableCollection<string> Associations { get; } = new ObservableCollection<string>();

        private string _selectedAssociation;
        public string SelectedAssociation
        {
            get => _selectedAssociation;
            set {
                _selectedAssociation = value;
                OnPropertyChanged();
                DetailViewModel.AssociationName = value;
            }
        }

        public AssociationDetailViewModel DetailViewModel { get; } = new AssociationDetailViewModel();

        public MainViewModel()
        {
            RefreshAssociations();
        }

        public void RefreshAssociations()
        {
            Associations.Clear();
            foreach (var assoc in _registryService.ListAssociations())
            {
                // Only show file extensions (start with .) and protocol names (contain ":")
                if (assoc.StartsWith(".") || assoc.Contains(":"))
                    Associations.Add(assoc);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 