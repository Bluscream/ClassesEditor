using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassesEditor.Services;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace ClassesEditor.ViewModels
{
    public class AssociationDetailViewModel : INotifyPropertyChanged
    {
        private readonly RegistryService _registryService = new RegistryService();
        private string _associationName;

        public string AssociationName
        {
            get => _associationName;
            set { _associationName = value; LoadDetails(); OnPropertyChanged(); }
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set { _displayName = value; OnPropertyChanged(); }
        }

        private string _defaultIcon;
        public string DefaultIcon
        {
            get => _defaultIcon;
            set { _defaultIcon = value; OnPropertyChanged(); }
        }

        private string _openCommand;
        public string OpenCommand
        {
            get => _openCommand;
            set { _openCommand = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public AssociationDetailViewModel()
        {
            SaveCommand = new RelayCommand(Save);
        }

        public string RegistryPath => $"HKEY_CLASSES_ROOT\\{AssociationName}";

        public void LoadDetails()
        {
            if (string.IsNullOrEmpty(AssociationName)) return;
            var details = _registryService.GetAssociationDetails(AssociationName);
            details.TryGetValue("", out var displayName); // Default value
            DisplayName = displayName as string;
            if (details.TryGetValue("SubKey:DefaultIcon", out _))
            {
                var iconDetails = _registryService.GetAssociationDetails($"{AssociationName}\\DefaultIcon");
                iconDetails.TryGetValue("", out var iconValue);
                DefaultIcon = iconValue as string;
            }
            else
            {
                DefaultIcon = null;
            }
            if (details.TryGetValue("SubKey:shell", out _))
            {
                var shellDetails = _registryService.GetAssociationDetails($"{AssociationName}\\shell\\open\\command");
                shellDetails.TryGetValue("", out var commandValue);
                OpenCommand = commandValue as string;
            }
            else
            {
                OpenCommand = null;
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(AssociationName)) return;
            // Save DisplayName (default value)
            _registryService.EditAssociation(AssociationName, new Dictionary<string, object> { [""] = DisplayName });
            // Save DefaultIcon
            if (!string.IsNullOrEmpty(DefaultIcon))
                _registryService.EditAssociation($"{AssociationName}\\DefaultIcon", new Dictionary<string, object> { [""] = DefaultIcon });
            // Save OpenCommand
            if (!string.IsNullOrEmpty(OpenCommand))
                _registryService.EditAssociation($"{AssociationName}\\shell\\open\\command", new Dictionary<string, object> { [""] = OpenCommand });
        }

        public void OpenInRegistryEditor()
        {
            // Open regedit at the association's path
            Process.Start("regedit.exe", $"/m \"HKEY_CLASSES_ROOT\\{AssociationName}\"");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 