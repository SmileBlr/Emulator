using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;

public class File : MonoBehaviour, IContextable
{
    public FileSystemInfo FileInfo;
    public string NameWithoutExtension;

[HideInInspector]  public ValidTypes Type;

    [SerializeField] private InputField Name;
    private Image InputImage;

    private bool isWaitSecondClick;
    private bool isRenaming;
    private Image selectImage;
    private Color basicColor;

    private void Awake()
    {
        selectImage = GetComponent<Image>();
        basicColor = selectImage.color;
        InputImage = Name.GetComponent<Image>();
        InputImage.enabled = false;
        selectImage.color = new Color(0, 0, 0, 0);
    }


    public virtual void SetInfo(FileSystemInfo fileInfo, ValidTypes type)
    {
        FileInfo = fileInfo;
        Type = type;
        Name.text = FileInfo.Name;
        NameWithoutExtension = Path.GetFileNameWithoutExtension(FileInfo.FullName);
    }

    public void HandleEnterKey()
    {
        if (InputImage.enabled) InputImage.enabled = false;
        else OpenRoot(true);
    }

    public void OpenRoot(bool isPermomentOpen = false)
    {
        if (InputManager.Instance.IsLastOpenInput)
        {
            ContextMenu.Instance.CloseContextMenu();

            if (!isWaitSecondClick && !isPermomentOpen)
            {
                SelectFile();
                StartCoroutine(WaitSecondClickRoutine());
            }
            else OpenFile();
        }
        else
        {
            ContextMenu.Instance.OpenContextMenu();
        }
    }
    
    protected virtual void OpenFile(){}

    public void UnselectFile()
    {
        Workplace.Instance.Buffer = null;

        selectImage.color = new Color(0, 0, 0, 0);

        InputImage.enabled = false;
        Name.interactable = false;

        isWaitSecondClick = false;
        StopCoroutine(WaitSecondClickRoutine());
    }

    public void InitializeAwake(Color basicColor)
    {
        selectImage = GetComponent<Image>();
        this.basicColor = basicColor;
        selectImage.color = new Color(0, 0, 0, 0);
    }

    public Color GetInitializeData() => basicColor;
    

    private IEnumerator WaitSecondClickRoutine()
    {
        isWaitSecondClick = true;

        yield return new WaitForSeconds(1);

        isWaitSecondClick = false;
    }

    public void SelectFile(bool offLast = true)
    {
        if (offLast && Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.UnselectFile();

        selectImage.color = basicColor;

        Workplace.Instance.Buffer = this;
    }

    public virtual void Create()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Copy()
    {
        if (Workplace.Instance.CopyBuffer != null) Destroy(Workplace.Instance.CopyBuffer.gameObject);

        ContextMenu.Instance.CloseContextMenu();

        var bufferObj = Instantiate(this.gameObject);
        bufferObj.GetComponent<File>().FileInfo = FileInfo;
        Workplace.Instance.CopyBuffer = bufferObj.GetComponent<File>();
        bufferObj.SetActive(false);
    }

    public virtual void Paste()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Remove()
    {
        ContextMenu.Instance.CloseContextMenu();
        Workplace.Instance.RemoveFileFromList(this);

        System.IO.File.Delete(FileInfo.FullName);
        if (System.IO.File.Exists(FileInfo.FullName + ".meta"))  System.IO.File.Delete(FileInfo.FullName + ".meta");

        Destroy(gameObject);
    }

    public void Open()
    {
        OpenFile();
    }

    public void Rename()
    {
        InputImage.enabled = true;
        Name.interactable = true;
        StartCoroutine(RenameRoutine());
        ContextMenu.Instance.CloseContextMenu();
    }

    private IEnumerator RenameRoutine()
    {
        Name.text = NameWithoutExtension;

        yield return new WaitWhile(() => InputImage.enabled);

        if (Workplace.Instance.IsValidName(this.gameObject, Name.text + FileInfo.Extension) && NameWithoutExtension != Name.text)
        {
            FileManager.Instance.RenameFile(this ,Name.text + FileInfo.Extension);
            NameWithoutExtension = Path.GetFileNameWithoutExtension(FileInfo.FullName);
            Name.text += FileInfo.Extension;
        }
        else
        {
            Name.text = FileInfo.Name;
        }
    }
}
