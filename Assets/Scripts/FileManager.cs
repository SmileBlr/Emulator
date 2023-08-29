using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class FileManager : MonoBehaviour, ICreatable
{
    public static FileManager Instance;

    private const string basicPath = @"D:\Unity\Emulator PC\Assets\Test Computer";
    private string currentPath;
    private DirectoryInfo currentDirectory;

    public delegate void ChangeHandler();
    public event ChangeHandler ChangeFolderEvent = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        currentPath = basicPath;

        LoadFilesAndDirectories();
        ChangeFolderEvent += LoadFilesAndDirectories;
    }

    public async void LoadFilesAndDirectories()
    {
        try
        {
            currentDirectory = new DirectoryInfo(currentPath);
            FileInfo[] files = new FileInfo[0];
            DirectoryInfo[] dirs = new DirectoryInfo[0];

            var tf = await Task.Run(() => files = currentDirectory.GetFiles());
            var td = await Task.Run(() => dirs = currentDirectory.GetDirectories());

            foreach (var dir in td)
            {
                Workplace.Instance.CreateNewDirectory(dir);
            }

            foreach (var file in tf)
            {
                Workplace.Instance.CreateNewFile(file);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void SetNewPath(string path) 
    {
        currentPath = path;
        ChangeFolderEvent?.Invoke();
    }

    public void BackToOldPath() 
    {
        if (currentPath != basicPath)
        {
            ContextMenu.Instance.CloseContextMenu();
            SetNewPath(currentDirectory.Parent.FullName);
        }
    }

    public void RenameFile(File sender, string newName)
    {
        var newPath = @$"{currentPath}\{newName}";
        var oldPath = sender.FileInfo.FullName;

        System.IO.File.Move(sender.FileInfo.FullName, newPath);
        System.IO.File.Delete(sender.FileInfo.FullName);

        sender.FileInfo = GetFileInfo(newPath);
        System.IO.File.Delete(oldPath);
    }

    public async Task<FileSystemInfo> PasteFile()
    {
        if (string.IsNullOrEmpty(Workplace.Instance.CopyBuffer.FileInfo.Extension))
        {
            List<string> pathsList = new List<string>();

            var diSource = new DirectoryInfo(Workplace.Instance.CopyBuffer.FileInfo.FullName);

            var tempPath = @$"{currentPath}\{Workplace.Instance.CopyBuffer.FileInfo.Name}";
            while (System.IO.Directory.Exists(tempPath)) tempPath += " Copy";
            var diTarget = new DirectoryInfo(tempPath);

            await Task.Run(() => CreateLists(diSource));
            await Task.Run(() => CopyAll(diSource, diTarget));

            async void CreateLists(DirectoryInfo source)
            {
                foreach (FileInfo fi in source.GetFiles())
                {
                    pathsList.Add(fi.FullName);
                }

                foreach (DirectoryInfo di in source.GetDirectories())
                {
                    pathsList.Add(di.FullName);
                    await Task.Run(() => CreateLists(di));
                }
            }

            async void CopyAll(DirectoryInfo source, DirectoryInfo target)
            {
                Directory.CreateDirectory(target.FullName);

                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    if (pathsList.Contains(fi.FullName))
                    {
                        pathsList.Remove(fi.FullName);

                        Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                        await Task.Run(() => fi.CopyTo(Path.Combine(target.FullName, fi.Name), true));
                    }
                }

                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    if (pathsList.Contains(diSourceSubDir.FullName))
                    {
                        pathsList.Remove(diSourceSubDir.FullName);

                        DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                        await Task.Run(() => CopyAll(diSourceSubDir, nextTargetSubDir));
                    }
                }
            }

            return diTarget;
        }
        else
        {
            var diSource = new FileInfo(Workplace.Instance.CopyBuffer.FileInfo.FullName);
            var diTarget = new FileInfo(@$"{currentPath}");

            var tempFileName = Path.GetFileNameWithoutExtension(diSource.FullName);
            while (System.IO.File.Exists($@"{currentPath}\{tempFileName}{diSource.Extension}")) tempFileName += " Copy";

            var file = await Task.Run(() => diSource.CopyTo(Path.Combine(diTarget.FullName, $@"{currentPath}\{tempFileName}{diSource.Extension}"), true));

            return file;
        }
    }


    public void CreateTextFile()
    {
        ContextMenu.Instance.CloseContextMenu();

        string basicName = "New Text";
        string name = basicName;
        var fileCount = 1;
        while (System.IO.File.Exists($@"{currentPath}\{name}.txt"))
        {
            name = $"{basicName} {fileCount}";
        }

        var path = @$"{currentPath}\{name}.txt";
        System.IO.File.CreateText(path);

        var info = new FileInfo(path);

        Workplace.Instance.CreateNewFile(info);
    }

    public void CreateFolder()
    {
        ContextMenu.Instance.CloseContextMenu();

        string basicName = "New Directory";
        string name = basicName;
        var fileCount = 1;
        while (System.IO.Directory.Exists($@"{currentPath}\{name}"))
        {
            name = $"{basicName} {fileCount}";
        }

        var path = @$"{currentPath}\{name}";
        var dir = System.IO.Directory.CreateDirectory(path);

        Workplace.Instance.CreateNewDirectory(dir);
    }

    private FileSystemInfo GetFileInfo(string path)
    {
 
        FileSystemInfo newInfo = System.IO.Directory.Exists(path)
            ? new DirectoryInfo(path) as FileSystemInfo
            : new FileInfo(path) as FileSystemInfo;
        return newInfo;
    }

    private void OnDisable()
    {
        ChangeFolderEvent -= LoadFilesAndDirectories;
    }
}
public interface ICreatable
{
    void CreateTextFile();
    void CreateFolder();
}
