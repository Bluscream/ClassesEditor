using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassesEditor.Services;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows;
using System;
using Microsoft.Win32;
using System.Collections.ObjectModel;

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

        private string _contentType;
        public string ContentType
        {
            get => _contentType;
            set { _contentType = value; OnPropertyChanged(); }
        }

        private string _perceivedType;
        public string PerceivedType
        {
            get => _perceivedType;
            set { _perceivedType = value; OnPropertyChanged(); }
        }

        private bool _alwaysShowExt;
        public bool AlwaysShowExt
        {
            get => _alwaysShowExt;
            set { _alwaysShowExt = value; OnPropertyChanged(); }
        }

        private bool _noRecentDocs;
        public bool NoRecentDocs
        {
            get => _noRecentDocs;
            set { _noRecentDocs = value; OnPropertyChanged(); }
        }

        private string _infoTip;
        public string InfoTip
        {
            get => _infoTip;
            set { _infoTip = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> OpenWithList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> OpenWithProgids { get; } = new ObservableCollection<string>();

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public AssociationDetailViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            DeleteCommand = new RelayCommand(Delete);
        }

        public string RegistryPath => $"HKEY_CLASSES_ROOT\\{AssociationName}";

        public void LoadDetails()
        {
            if (string.IsNullOrEmpty(AssociationName)) return;
            var details = _registryService.GetAssociationDetails(AssociationName);
            details.TryGetValue("", out var displayName); // Default value
            DisplayName = displayName as string;
            details.TryGetValue("Content Type", out var contentType);
            ContentType = contentType as string;
            details.TryGetValue("PerceivedType", out var perceivedType);
            PerceivedType = perceivedType as string;
            details.TryGetValue("AlwaysShowExt", out var alwaysShowExt);
            AlwaysShowExt = alwaysShowExt != null;
            details.TryGetValue("NoRecentDocs", out var noRecentDocs);
            NoRecentDocs = noRecentDocs != null;
            details.TryGetValue("InfoTip", out var infoTip);
            InfoTip = infoTip as string;
            // OpenWithList
            OpenWithList.Clear();
            using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($"{AssociationName}\\OpenWithList"))
            {
                if (key != null)
                {
                    foreach (var subKey in key.GetSubKeyNames())
                        OpenWithList.Add(subKey);
                }
            }
            // OpenWithProgids
            OpenWithProgids.Clear();
            using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($"{AssociationName}\\OpenWithProgids"))
            {
                if (key != null)
                {
                    foreach (var valueName in key.GetValueNames())
                        OpenWithProgids.Add(valueName);
                }
            }
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
            var values = new Dictionary<string, object> { [""] = DisplayName };
            if (!string.IsNullOrEmpty(ContentType))
                values["Content Type"] = ContentType;
            if (!string.IsNullOrEmpty(PerceivedType))
                values["PerceivedType"] = PerceivedType;
            if (AlwaysShowExt)
                values["AlwaysShowExt"] = "";
            if (NoRecentDocs)
                values["NoRecentDocs"] = "";
            if (!string.IsNullOrEmpty(InfoTip))
                values["InfoTip"] = InfoTip;
            _registryService.EditAssociation(AssociationName, values);
            // OpenWithList
            using (var key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($"{AssociationName}\\OpenWithList"))
            {
                if (key != null)
                {
                    foreach (var subKey in key.GetSubKeyNames())
                        key.DeleteSubKey(subKey, false);
                    foreach (var exe in OpenWithList)
                        key.CreateSubKey(exe);
                }
            }
            // OpenWithProgids
            using (var key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($"{AssociationName}\\OpenWithProgids"))
            {
                if (key != null)
                {
                    foreach (var valueName in key.GetValueNames())
                        key.DeleteValue(valueName, false);
                    foreach (var progid in OpenWithProgids)
                        key.SetValue(progid, new byte[0], Microsoft.Win32.RegistryValueKind.Binary);
                }
            }
            if (!string.IsNullOrEmpty(DefaultIcon))
                _registryService.EditAssociation($"{AssociationName}\\DefaultIcon", new Dictionary<string, object> { [""] = DefaultIcon });
            if (!string.IsNullOrEmpty(OpenCommand))
                _registryService.EditAssociation($"{AssociationName}\\shell\\open\\command", new Dictionary<string, object> { [""] = OpenCommand });
        }

        public void Delete()
        {
            if (string.IsNullOrEmpty(AssociationName)) return;
            _registryService.DeleteAssociation(AssociationName);
            // Optionally, clear fields or notify user
        }

        public void OpenInRegistryEditor()
        {
            try
            {
                // Set the LastKey value so regedit opens at the correct key
                string lastKey = $"Computer\\HKEY_CLASSES_ROOT\\{AssociationName}";
                using (var regeditKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit"))
                {
                    regeditKey.SetValue("LastKey", lastKey);
                }
                // Open regedit (no arguments)
                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Registry Editor:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Methods to add/remove OpenWithList and OpenWithProgids
        public void AddOpenWith(string exe) { if (!OpenWithList.Contains(exe)) OpenWithList.Add(exe); }
        public void RemoveOpenWith(string exe) { if (OpenWithList.Contains(exe)) OpenWithList.Remove(exe); }
        public void AddOpenWithProgid(string progid) { if (!OpenWithProgids.Contains(progid)) OpenWithProgids.Add(progid); }
        public void RemoveOpenWithProgid(string progid) { if (OpenWithProgids.Contains(progid)) OpenWithProgids.Remove(progid); }
    }
} 