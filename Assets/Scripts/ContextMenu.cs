using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenu : MonoBehaviour, IContextable
{
    public static ContextMenu Instance;
    [Header("Total")]
    [SerializeField] private RectTransform ContextMenuObj;
    [SerializeField] private RectTransform CreateMenuObj;
    [Header("Parts")]
    [SerializeField] private ContextMenuPart CreateButton;
    [SerializeField] private ContextMenuPart CopyButton;
    [SerializeField] private ContextMenuPart PasteButton;
    [SerializeField] private ContextMenuPart RemoveButton;
    [SerializeField] private ContextMenuPart OpenButton;
    [SerializeField] private ContextMenuPart RenameButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void OpenContextMenu(bool isFile = false)
    {
        CloseCreateMenu();

        Vector3 position = Input.mousePosition;
        position.y = position.y * 0.9f - ContextMenuObj.rect.height < 0 ? position.y + ContextMenuObj.rect.height / 2 : position.y - ContextMenuObj.rect.height / 2;
        position.x = position.x + ContextMenuObj.rect.width > Screen.width ? position.x - ContextMenuObj.rect.width/2 :position.x + ContextMenuObj.rect.width / 2;

        ContextMenuObj.position = position;
        ContextMenuObj.gameObject.SetActive(true);

        switch (isFile)
        {
            case true:
                CreateButton.DisableButton();
                CopyButton.EnableButton();
                PasteButton.DisableButton();
                RemoveButton.EnableButton();
                OpenButton.EnableButton();
                RenameButton.EnableButton();
                break;

            case false:
                CreateButton.EnableButton();
                CopyButton.DisableButton();
                RemoveButton.DisableButton();
                OpenButton.DisableButton();
                RenameButton.DisableButton();
                if (Workplace.Instance.CopyBuffer != null) PasteButton.EnableButton();
                else PasteButton.DisableButton();
                break;
        }
    }

    public void CloseContextMenu() 
    {
        ContextMenuObj.gameObject.SetActive(false);
        CloseCreateMenu();
    }

    public void Create()
    {
        OpenCreateContextMenu();
    }

    public void Copy()
    {
        if (Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.Copy();
    }

    public void Paste()
    {
        if (Workplace.Instance.CopyBuffer != null) Workplace.Instance.Paste();
    }

    public void Remove()
    {
        if (Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.Remove();
    }

    public void Open()
    {
        if (Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.OpenRoot(true);
    }
    public void Rename()
    {
        if (Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.Rename();
    }

    private void OpenCreateContextMenu()
    {
        Vector3 position = ContextMenuObj.position;
        position.y = CreateButton.GetComponent<RectTransform>().position.y - CreateMenuObj.rect.height / 4;
        position.x = position.x + ContextMenuObj.rect.width/2+ CreateMenuObj.rect.width > Screen.width ?
            position.x - ContextMenuObj.rect.width / 2 - CreateMenuObj.rect.width/2 : position.x + ContextMenuObj.rect.width / 2 + CreateMenuObj.rect.width/2;

        CreateMenuObj.position = position;
        CreateMenuObj.gameObject.SetActive(true);
    }

    private void CloseCreateMenu() => CreateMenuObj.gameObject.SetActive(false);
}

interface IContextable
{
    public void Create();
    public void Copy();
    public void Paste();
    public void Remove();
    public void Open();
    public void Rename();
}
