using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

public class Workplace : MonoBehaviour
{
    public static Workplace Instance;

    public File Buffer;
    public File CopyBuffer;

    [SerializeField] private Transform Content;

    [SerializeField] private GameObject FolderPref;
    [SerializeField] private GameObject TextPref;
    [SerializeField] private GameObject ImgPref;
    [SerializeField] private GameObject OtherPref;

    private GridLayoutGroup gridLayout;
    private List<File> allFilesList;
    private int GridRowCount => (int)Math.Truncate((double)(Screen.width - gridLayout.padding.left - gridLayout.padding.right) / (gridLayout.cellSize.x + gridLayout.spacing.x));

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        allFilesList = new List<File>();
        gridLayout = Content.GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        FileManager.Instance.ChangeFolderEvent += ClearWorkPlace;
    }

    public void CreateNewFile(FileInfo file)
    {
        var type = CheckType(file);

        var objForSpawn = type switch
        {
            ValidTypes.Text => TextPref,
            ValidTypes.Image => ImgPref,
            _ => OtherPref
        };

        var obj = Instantiate(objForSpawn, Content);
        allFilesList.Add(obj.GetComponent<File>());
        obj.GetComponent<File>().SetInfo(file,type);
    }

    public void CreateNewDirectory(DirectoryInfo dir)
    {
        var objForSpawn = FolderPref;

        var obj = Instantiate(objForSpawn, Content);
        allFilesList.Add(obj.GetComponent<File>());
        obj.GetComponent<File>().SetInfo(dir, ValidTypes.Folder);
    }

    public void MoveOnNextFile()
    {
        if (Buffer != null)
        {
            ContextMenu.Instance.CloseContextMenu();

            var newIndex = Buffer.transform.GetSiblingIndex(); 
            newIndex = newIndex == Content.childCount - 1 ? 0 : ++newIndex;

            SelectByIndex(newIndex);
        }
    }
    public void MoveOnLastFile()
    {
        if (Buffer != null)
        {
            ContextMenu.Instance.CloseContextMenu();

            var newIndex = Buffer.transform.GetSiblingIndex();
            newIndex = newIndex == 0 ? Content.childCount - 1 : --newIndex;

            SelectByIndex(newIndex);
        }
    }
    public void MoveOnUpFile()
    {
        if (Buffer != null)
        {
            ContextMenu.Instance.CloseContextMenu();

            var newIndex = Buffer.transform.GetSiblingIndex() - GridRowCount;
            newIndex = newIndex < 0 ? 0 : newIndex;

            SelectByIndex(newIndex);
        }
    }
    public void MoveOnDownFile()
    {
        if (Buffer != null)
        {
            ContextMenu.Instance.CloseContextMenu();

            var newIndex = Buffer.transform.GetSiblingIndex() + GridRowCount;
            newIndex = newIndex >= Content.childCount ? Content.childCount - 1 : newIndex;

            SelectByIndex(newIndex);
        }
    }

    public async void Paste()
    {
        if (CopyBuffer != null)
        {
            ContextMenu.Instance.CloseContextMenu();
            var info =  await Task.Run((() => FileManager.Instance.PasteFile()));

            var obj = Instantiate(CopyBuffer.gameObject, Content);
            obj.SetActive(true);

            var file = obj.GetComponent<File>();
            file.FileInfo = info;
            file.InitializeAwake(CopyBuffer.GetInitializeData());
            file.SetInfo(info, CheckType(info));
        }
    }

    public bool IsValidName(GameObject sender, string name)
    {
        foreach (var file in allFilesList)
        {
            if (file.gameObject != sender && name == file.FileInfo.Name)
            {
                return false;
            }
        }

        return true;
    }

    public void RemoveFileFromList(File sender)
    {
        if (allFilesList.Contains(sender)) allFilesList.Remove(sender);
    }

    private void SelectByIndex(int index)
    {
        Buffer.UnselectFile();
        Buffer = Content.GetChild(index).GetComponent<File>();
        Buffer.SelectFile(false);
    }

    private void ClearWorkPlace()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }

        allFilesList.Clear();
    }

    private ValidTypes CheckType(FileSystemInfo info)
    {
        var type = info.Extension switch
        {
            ".txt" => ValidTypes.Text,
            ".png" => ValidTypes.Image,
            ".jpeg" => ValidTypes.Image,
            "" => ValidTypes.Folder,
            _ => ValidTypes.Other
        };

        return type;
    }

    private void OnDisable()
    {
        FileManager.Instance.ChangeFolderEvent -= ClearWorkPlace;
    }
}

public enum ValidTypes
{
    Text,
    Image,
    Folder,
    Other
}
