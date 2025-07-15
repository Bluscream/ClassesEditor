using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassesEditor.Services;

namespace ClassesEditor.ViewModels
{
    public enum AssociationFilterMode { FileType, Protocol }

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly RegistryService _registryService = new RegistryService();
        private readonly AssociationFilterMode _filterMode;

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

        public MainViewModel(AssociationFilterMode filterMode = AssociationFilterMode.FileType)
        {
            _filterMode = filterMode;
            RefreshAssociations();
        }

        public void RefreshAssociations()
        {
            Associations.Clear();
            foreach (var assoc in _registryService.ListAssociations())
            {
                if (_filterMode == AssociationFilterMode.FileType && assoc.StartsWith("."))
                    Associations.Add(assoc);
                else if (_filterMode == AssociationFilterMode.Protocol && !assoc.StartsWith("."))
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