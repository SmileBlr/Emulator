using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFile : File
{
    protected override void OpenFile()
    {
        TextView.Instance.ShowText(FileInfo.FullName);
    }
}
