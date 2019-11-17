using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomDebug
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
