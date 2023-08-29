using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageView : MonoBehaviour
{
    public static ImageView Instance;
    [SerializeField] private GameObject viewZone;
    [SerializeField] private RawImage viewImg;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void ShowImage(Texture2D img)
    {
        viewImg.texture = img;
        viewZone.SetActive(true);
    }
}
