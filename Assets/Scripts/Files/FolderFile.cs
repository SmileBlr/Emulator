using System.IO;
using UnityEngine;

public class FolderFile : File
{
    protected override void OpenFile() => FileManager.Instance.SetNewPath(FileInfo.FullName);

    public override void Remove()
    {
        ContextMenu.Instance.CloseContextMenu();
        Workplace.Instance.RemoveFileFromList(this);

        System.IO.File.Delete(FileInfo.FullName + ".meta");
        (FileInfo as DirectoryInfo).Delete(true);
        Destroy(gameObject);
    }
}
