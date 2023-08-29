using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuPart : MonoBehaviour
{
    private Button button;
    private Color basicColor;
    private Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        basicColor = image.color;
    }

    public void DisableButton()
    {
        button.enabled = false;
        image.color = Color.white;
    }
    public void EnableButton()
    {
        button.enabled = true;
        image.color = basicColor;
    }
}
