using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassesEditor.Services;
using System.Diagnostics;

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
            Debug.WriteLine($"MainViewModel created for mode: {_filterMode}");
            RefreshAssociations();
        }

        public void RefreshAssociations()
        {
            Debug.WriteLine($"Refreshing associations for mode: {_filterMode}");
            Associations.Clear();
            int count = 0;
            foreach (var assoc in _registryService.ListAssociations())
            {
                if (_filterMode == AssociationFilterMode.FileType && assoc.StartsWith("."))
                {
                    Associations.Add(assoc);
                    count++;
                }
                else if (_filterMode == AssociationFilterMode.Protocol && !assoc.StartsWith("."))
                {
                    Associations.Add(assoc);
                    count++;
                }
            }
            Debug.WriteLine($"Found {count} associations for mode: {_filterMode}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 