using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MobTest
{
    class FolderHandler
    {
        IReadOnlyList<IStorageItem> folderElementsList;
        public string FolderPath { get; private set; }
        public string[] FolderElements { get; private set; }

        internal async void SelectFolder()
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.HomeGroup;
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            FolderPath = folder.Path;
            if (folder != null)
            {
                MessageBox.Show("Picked folder: " + FolderPath, "Service information", MessageBoxButton.OK);
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                FolderElements = await folder.GetItemsAsync();
            }
            else
            {
                MessageBox.Show("Operation cancelled.", "Service information", MessageBoxButton.OK);
            }
        }

        internal bool IsFolderEmpty()
        {
            return FolderElements.Length == 0;
        }

        public void CreateServiceFile(string _full_file_path)
        {
            FileStream serviceFileCreator = File.Create(_full_file_path);
            serviceFileCreator.Close();
        }

        public string[] ReadTextFile(string _full_file_path)
        {
            List<string> textStrings = new List<string>();
            TextReader statementFileReader = new StreamReader(_full_file_path);
            {
                string temp;
                while ((temp = statementFileReader.ReadLine()) != null)
                {
                    textStrings.Add(temp);
                }
            }
            return textStrings.ToArray();
        }
    }
}
