using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace GoogleAnalyticsClientDotNet.IO
{
    public static class StorageHelper
    {
        public static StorageFolder GetApplicationDataFolder(ApplicationDataLocation location)
        {
            StorageFolder folder = null;
            switch (location)
            {
                case ApplicationDataLocation.Local:
                    folder = ApplicationData.Current.LocalFolder;
                    break;
                case ApplicationDataLocation.Roaming:
                    folder = ApplicationData.Current.RoamingFolder;
                    break;
                case ApplicationDataLocation.Temp:
                    folder = ApplicationData.Current.TemporaryFolder;
                    break;
                case ApplicationDataLocation.Install:
                    folder = Package.Current.InstalledLocation;
                    break;
            }

            return folder;
        }

        public static async Task<IInputStream> OpenSequentialRead(string filePath, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            Debug.Assert(!string.IsNullOrEmpty(filePath));
            StorageFolder folder = GetApplicationDataFolder(location);
            var file = await folder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
            if (file == null)
            {
                return null;
            }

            return await file.OpenSequentialReadAsync();
        }

        public static async Task<Stream> OpenReadStream(string filePath, bool forceCreate, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            StorageFolder folder = GetApplicationDataFolder(location);

            try
            {
                StorageFile file = null;
                if (forceCreate)
                {
                    file = await folder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
                }
                else
                {
                    file = await folder.GetFileAsync(filePath);
                }

                if (file == null)
                {
                    return null;
                }

                return await file.OpenStreamForReadAsync();
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        public static async Task<Stream> OpenWriteStream(string filePath, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            StorageFolder folder = GetApplicationDataFolder(location);
            folder = await folder.CreateFolderLazy(filePath);
            var fileName = Path.GetFileName(filePath);
            CreationCollisionOption createOption = (overwrite ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);

            try
            {
                var file = await folder.CreateFileAsync(fileName, createOption);
                if (file == null)
                {
                    return null;
                }

                return await file.OpenStreamForWriteAsync();
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        public static async Task<StorageFile> OpenFile(string filePath, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            StorageFolder folder = GetApplicationDataFolder(location);
            folder = await folder.CreateFolderLazy(filePath);
            var fileName = Path.GetFileName(filePath);
            CreationCollisionOption createOption = (overwrite ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);
            try
            {
                return await folder.CreateFileAsync(fileName, createOption);
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        public static async Task<long> GetFreeSapce(ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var storageFolder = GetApplicationDataFolder(location);
            var retrivedProperties = await storageFolder.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" });
            return Convert.ToInt64(retrivedProperties["System.FreeSpace"]);
        }

        public static async Task<StorageFolder> CreateFolderLazy(this StorageFolder storage, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return storage;
            }

            const char directorySeparatorChar = '\\';
            string[] subDirectoies = directoryName.Split(new char[] { directorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string path = string.Empty;
            StorageFolder parentFolder = storage;
            StorageFolder childFolder = null;

            for (int i = 0; i < subDirectoies.Length; i++)
            {
                path = subDirectoies[i];
                try
                {
                    childFolder = await parentFolder.GetFolderAsync(path);
                }
                catch (FileNotFoundException)
                {
                    childFolder = null;
                }

                if (null == childFolder)
                {
                    parentFolder = await parentFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);
                }
                else
                {
                    parentFolder = childFolder;
                }
            }

            return parentFolder;
        }

        public static async Task DeleteFolder(string folderName, bool needKeepFodler = false, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var appDataFolder = GetApplicationDataFolder(location);
            try
            {
                var folder = await appDataFolder.GetFolderAsync(folderName);
                if (folder == null)
                {
                    return;
                }

                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                if (needKeepFodler)
                {
                    await CreateFolderLazy(appDataFolder, folderName);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public static async Task<bool> DeleteFile(string fileName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                var file = await GetFile(fileName, location);
                if (file == null)
                {
                    return false;
                }

                await file.DeleteAsync();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static async Task<string> GetFilePath(string filePath, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var storage = GetApplicationDataFolder(location);
            var file = await storage.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
            return file.Path;
        }

        public static async Task<bool> ExistsFolder(string foldername, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var folder = GetApplicationDataFolder(location);

            try
            {
                await folder.GetFolderAsync(foldername);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static async Task<bool> ExistsFile(string fileName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                var file = await GetFile(fileName, location);
                return (file != null);
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static async Task<StorageFile> GetFile(string fileName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            StorageFolder storageFolder = GetApplicationDataFolder(location);
            var item = await storageFolder.TryGetItemAsync(fileName);
            if (item != null && item.IsOfType(StorageItemTypes.File))
            {
                return item as StorageFile;
            }
            else
            {
                return null;
            }
        }

        public static async Task<IReadOnlyList<StorageFile>> GetFilesInFolder(List<string> fileTypeFilter, StorageFolder folder)
        {
            if (folder == null || fileTypeFilter == null || fileTypeFilter.Count == 0)
            {
                return null;
            }
            else
            {
                try
                {
                    QueryOptions queryOptions = new QueryOptions();
                    foreach (var item in fileTypeFilter)
                    {
                        if (item.IndexOf(".") == 0)
                        {
                            queryOptions.FileTypeFilter.Add(item);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    StorageFileQueryResult results = folder.CreateFileQueryWithOptions(queryOptions);
                    return await results.GetFilesAsync();
                }
                catch (IOException)
                {
                    return null;
                }
                catch (UnauthorizedAccessException)
                {
                    return null;
                }
            }
        }

        public static async Task<StorageFile> GetFilesInFolder(string fileName, StorageFolder root)
        {
            StorageFile file = null;
            StorageFolder folder = root;

            var items = await folder.GetItemsAsync();

            foreach (var item in items)
            {
                if (item.GetType() == typeof(StorageFile))
                {
                    if (item.Name == fileName)
                    {
                        file = item as StorageFile;
                        break;
                    }
                }
                else
                {
                    file = await GetFilesInFolder(fileName, item as StorageFolder);
                }
            }
            return file;
        }

        public static async Task<StorageFolder> GetFolder(string folderName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                StorageFolder storageFolder = GetApplicationDataFolder(location);
                var item = await storageFolder.TryGetItemAsync(folderName);
                if (item != null && item.IsOfType(StorageItemTypes.Folder))
                {
                    return item as StorageFolder;
                }
                else
                {
                    return await storageFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                }
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
        public static async Task<bool> WriteFileAsync(StorageFile targetFile, IBuffer fileContent)
        {
            try
            {
                CachedFileManager.DeferUpdates(targetFile);
                await FileIO.WriteBufferAsync(targetFile, fileContent);
            }
            catch (Exception)
            {
                return false;
            }

            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(targetFile);
            return status == FileUpdateStatus.Complete;
        }
    }
}