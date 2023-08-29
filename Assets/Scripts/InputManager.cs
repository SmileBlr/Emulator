using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public bool IsLastOpenInput;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return))
        {
            IsLastOpenInput = true;

            if (Input.GetKeyDown(KeyCode.Return) && Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.HandleEnterKey();
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            IsLastOpenInput = false;

            UIRaycastHandle();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            FileManager.Instance.BackToOldPath();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Workplace.Instance.MoveOnLastFile();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Workplace.Instance.MoveOnNextFile();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Workplace.Instance.MoveOnUpFile();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Workplace.Instance.MoveOnDownFile();
        }
        else if (Input.GetKeyDown(KeyCode.Delete) && Workplace.Instance.Buffer != null)
        {
            Workplace.Instance.Buffer.Remove();
        }
    }

    private void UIRaycastHandle()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            var file = raycastResults[0].gameObject.GetComponent<File>();
            if (file != null)
            {
                file.SelectFile();
                ContextMenu.Instance.OpenContextMenu(true);
            }
            else
            {
                if(Workplace.Instance.Buffer != null) Workplace.Instance.Buffer.UnselectFile();
                ContextMenu.Instance.OpenContextMenu();
            }
        }
    }
}
