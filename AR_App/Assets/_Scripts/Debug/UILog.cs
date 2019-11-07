using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.IO;
using CustomDebug;

public class UILog : MonoBehaviour
{
    #region Show in editor

    [Header("   Colors:")]
    [Header("Format:")]
    [SerializeField] private Color callerColor;
    [SerializeField] private Color baseColor;

    [Header("")]
    [SerializeField] private int indentPercent;

    [SerializeField] private TextMeshProUGUI logText;

    #endregion
    #region Hide in editor

    public static UILog Instance;
    
    #endregion

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            WriteLn("================= Debug Logs =================", Color.yellow, "");
        }
        else
        {
            Destroy(this);
        }
    }
    private void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }


    public void WriteLn(string line, [CallerMemberName] string callerName = "")
    {
        WriteLn(line, baseColor, callerName);
    }
    public void WriteLn(string line, Color color, [CallerMemberName] string callerFunction = "")
    {
        Write(line, color, callerFunction);
        logText.text += "\n\n";
    }
    public void Write(string line, [CallerMemberName] string callerName = "")
    {
        Write(line, baseColor, callerName);
    }
    public void Write(string line, Color color, [CallerMemberName] string callerFunction = "")
    {
        if (callerFunction == string.Empty)
        {
            logText.text += $"{line.Color(color)}";
        }
        else
        {
            logText.text += $"{callerFunction.Italic().Color(callerColor)}\n{line.Color(color).Indent(indentPercent)}";
        }
    }
    public void Clear()
    {
        logText.text = "";
        WriteLn("================= Logs Cleared =================", Color.yellow, "");
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            Write(condition + "\n", Color.red);
            WriteLn(stackTrace, Color.red);
        }
        else if(type == LogType.Warning)
        {
            Write(condition + "\n", Color.yellow);
            WriteLn(stackTrace, Color.yellow);
        }
    }

}
