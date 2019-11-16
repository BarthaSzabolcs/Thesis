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

    [Header("Log Types:")]
    [SerializeField] bool exceptionLogging;
    [SerializeField] bool warningLogging;
    [SerializeField] bool debugLogging;

    [Header("   Colors:")]
    [Header("Format:")]
    [SerializeField] private Color consoleMessageColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color logColor;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color exceptionColor;

    [Header("")]
    [SerializeField] private int indentPercent;

    [SerializeField] private TextMeshProUGUI logText;

    #endregion
    #region Hide in editor

    public static UILog Instance;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            WriteLn("================= Debug Logs =================", consoleMessageColor, "");
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

    #endregion

    public void WriteLn(string line, [CallerMemberName] string callerName = "")
    {
        WriteLn(line, normalColor, callerName);
    }
    public void WriteLn(string line, Color color, [CallerMemberName] string callerFunction = "")
    {
        Write(line, color, callerFunction);
        logText.text += "\n\n";
    }
    public void Write(string line, [CallerMemberName] string callerName = "")
    {
        Write(line, normalColor, callerName);
    }
    public void Write(string line, Color color, [CallerMemberName] string callerFunction = "")
    {
        if (callerFunction == string.Empty)
        {
            logText.text += $"{line.Color(color)}";
        }
        else
        {
            logText.text += $"{callerFunction.Italic().Color(color)}\n{line.Color(color).Indent(indentPercent)}";
        }
    }
    public void Clear()
    {
        logText.text = "";
        WriteLn("================= Logs Cleared =================", consoleMessageColor, string.Empty);
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert)
        {
            if (exceptionLogging)
            {
                Write(condition + "\n", exceptionColor, string.Empty);
                WriteLn(stackTrace, exceptionColor, string.Empty);
            }
        }
        else if(type == LogType.Warning)
        {
            if (warningLogging)
            {
                WriteLn(condition, warningColor, string.Empty);
            }
        }
        else if(type == LogType.Log)
        {
            if (debugLogging)
            {
                WriteLn(condition, logColor, string.Empty);
            }
        }
    }

}
