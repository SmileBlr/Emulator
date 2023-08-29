using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ImageFile : File
{
    private Texture2D imageForView;

    public override void SetInfo(System.IO.FileSystemInfo fileInfo, ValidTypes type)
    {
        base.SetInfo(fileInfo, type);

        Invoke("LoadImg",Time.deltaTime*2);
    }

    protected override void OpenFile()
    {
        if(imageForView != null) ImageView.Instance.ShowImage(imageForView);
    }

    private async void LoadImg()
    {
        if (System.IO.File.Exists(FileInfo.FullName))
        {
            var rawData = await Task.Run(() => System.IO.File.ReadAllBytes(FileInfo.FullName));
            imageForView = new Texture2D(2, 2);
            imageForView.LoadImage(rawData);
        }
        else Invoke("LoadImg",1f);
    }
}
