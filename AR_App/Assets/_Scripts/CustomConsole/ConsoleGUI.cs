using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.IO;
using UnityEngine.UI;
using System.Linq;

namespace CustomConsole
{
    public class ConsoleGUI : MonoBehaviour
    {
        #region Show in editor

        [SerializeField] bool minimizeOnStart;

        [Header("Logs:")]
        [SerializeField] bool showException;
        [SerializeField] bool showWarning;
        [SerializeField] bool showDebug;

        [SerializeField] bool parseTags;
        [SerializeField] ConsoleTag[] logTags;

        [Header("Format:")]
        [SerializeField] private int callerMemberIndentation;

        [Header("   Colors:")]
        [SerializeField] private Color callerMemberColor;
        [SerializeField] private Color consoleMessageColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color warningColor;
        [SerializeField] private Color exceptionColor;

        [Header("Required Components")]
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private RectTransform logTextTransform;
        [SerializeField] private RectTransform consoleTransform;
        [SerializeField] private Image scrollbarHandle;

        #endregion
        #region Hide in editor

        public static ConsoleGUI Instance;
        private float viewportHeight;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                WriteLn("================= Debug Logs =================", consoleMessageColor, "");
                Debug.Log($"#succes# ...TEST...");
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            viewportHeight = consoleTransform.rect.height;

            if (minimizeOnStart)
            {
                ToggleConsole();
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

        public void ToggleConsole()
        {
            if (consoleTransform.rect.height > 1)
            {
                consoleTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                scrollbarHandle.enabled = false;
            }
            else
            {
                consoleTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, viewportHeight);
                scrollbarHandle.enabled = true;
            }
        }

        public void WriteLn(string line, [CallerMemberName] string callerName = "", LogType type = LogType.Log)
        {
            WriteLn(line, GetTypeColor(type), callerName);
        }
        public void WriteLn(string line, Color color, [CallerMemberName] string callerFunction = "")
        {
            Write(line, color, callerFunction);
            logText.text += "\n\n";
        }

        public void Write(string line, [CallerMemberName] string callerName = "", LogType type = LogType.Log)
        {
            Write(line, GetTypeColor(type), callerName);
        }
        public void Write(string line, Color color, [CallerMemberName] string callerFunction = "")
        {
            if (callerFunction == string.Empty)
            {
                logText.text += $"{ line.Color(color).Indent(callerMemberIndentation) }";
            }
            else
            {
                logText.text += $"{ callerFunction.Italic().Color(callerMemberColor) }\n{ line.Color(color).Indent(callerMemberIndentation) }";
            }

            var size = logText.GetPreferredValues();
            logTextTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
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
                if (showException)
                {
                    var color = parseTags ? ParseTag(condition) : GetTypeColor(type);

                    Write(condition + "\n", GetTypeColor(type), string.Empty);
                    WriteLn(stackTrace, GetTypeColor(type), string.Empty);
                }
            }
            else if(type == LogType.Warning)
            {
                if (showWarning)
                {
                    var color = parseTags ? ParseTag(condition) : GetTypeColor(type);

                    WriteLn(condition, GetTypeColor(type), string.Empty);
                }
            }
            else if(type == LogType.Log)
            {
                if (showDebug)
                {
                    var color = parseTags ? ParseTag(condition) : GetTypeColor(type);

                    WriteLn(condition, GetTypeColor(type), string.Empty);
                }
            }
        }

        private Color GetTypeColor(LogType log)
        {
            switch (log)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    return exceptionColor;

                case LogType.Warning:
                    return warningColor;

                case LogType.Log:
                    default:
                    return normalColor;
            }
        }
        private Color ParseTag(string str)
        {
            // ToDo - Fix
            var tagSTR = str.Split('#');

            var logTag = logTags.FirstOrDefault(x => x.tag == tag);

            return logTag != null ? logTag.color : normalColor;
        }
    }
}
