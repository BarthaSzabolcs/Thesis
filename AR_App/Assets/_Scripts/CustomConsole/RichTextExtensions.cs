using UnityEngine;

namespace CustomConsole
{
    public static class RichTextExtensions
    {
        public static string Color(this string s, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{ s }</color>";
        }

        public static string Indent(this string s, int value)
        {
            return $"<indent={value}%>{ s }</indent>";
        }

        public static string Bold(this string s)
        {
            return $"<b>{ s }</b>";
        }

        public static string Italic(this string s)
        {
            return $"<i>{ s }</i>";
        }

    }
}
