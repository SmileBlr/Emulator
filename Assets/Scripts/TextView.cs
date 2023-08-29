using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextView : MonoBehaviour
{
    public static TextView Instance;
    [SerializeField] private GameObject viewZone;
    [SerializeField] private InputField inputField;

    private string path = string.Empty;
    private bool isNeedWrite;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public async void ShowText(string path)
    {
        this.path = path;
        var newPath = string.Concat(path);
        await Task.Run(() => ReadString(newPath));

        viewZone.SetActive(true);
    }

    public async void WriteString()
    {
        if (!string.IsNullOrEmpty(path) && isNeedWrite)
        {
            isNeedWrite = false;

            using StreamWriter writeText = new StreamWriter(path);
            await writeText.WriteLineAsync(inputField.text);

            path = null;
        }
    }

    private async void ReadString(string path)
    {
        StreamReader reader = new StreamReader(path);
        var text = await Task.Run(() => reader.ReadToEnd());
        inputField.text = text;
        isNeedWrite = true;
        await Task.Run(() => reader.Close());
    }

    private void OnApplicationQuit()
    {
        WriteString();
    }
}
