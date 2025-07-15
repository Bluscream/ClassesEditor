using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace ClassesEditor.Services
{
    public class RegistryService
    {
        private const string ClassesRoot = "HKEY_CLASSES_ROOT";

        // List all filetype and protocol associations (top-level keys)
        public IEnumerable<string> ListAssociations()
        {
            using (var key = Registry.ClassesRoot)
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    yield return subKeyName;
                }
            }
        }

        // Get details for a specific association (returns all values and subkeys)
        public Dictionary<string, object> GetAssociationDetails(string associationName)
        {
            var result = new Dictionary<string, object>();
            using (var key = Registry.ClassesRoot.OpenSubKey(associationName))
            {
                if (key == null) return result;
                foreach (var valueName in key.GetValueNames())
                {
                    result[valueName] = key.GetValue(valueName);
                }
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    result[$"SubKey:{subKeyName}"] = null; // Placeholder for subkey navigation
                }
            }
            return result;
        }

        // Add a new association (filetype or protocol)
        public bool AddAssociation(string associationName, Dictionary<string, object> values)
        {
            try
            {
                using (var key = Registry.ClassesRoot.CreateSubKey(associationName))
                {
                    if (key == null) return false;
                    foreach (var kvp in values)
                    {
                        key.SetValue(kvp.Key, kvp.Value);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return false;
            }
        }

        // Edit an existing association (overwrite values)
        public bool EditAssociation(string associationName, Dictionary<string, object> values)
        {
            try
            {
                using (var key = Registry.ClassesRoot.OpenSubKey(associationName, writable: true))
                {
                    if (key == null) return false;
                    foreach (var kvp in values)
                    {
                        key.SetValue(kvp.Key, kvp.Value);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return false;
            }
        }

        // Delete an association
        public bool DeleteAssociation(string associationName)
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(associationName, throwOnMissingSubKey: false);
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                return false;
            }
        }
    }
} 