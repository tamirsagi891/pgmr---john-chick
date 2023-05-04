// Define the following scripting define symbols to enable debug functionality:
// DEBUG_LOG

using UnityEngine;
using System;
using System.Diagnostics;
using Avrahamy.Utils;
// using UnityEditor.Graphs;

namespace Avrahamy
{
    public enum LogTag
    {
        Default = 0,
        Gameplay = 1,
        Audio = 20,
        UI = 21,
        Input = 22,
        Messages = 30,
        Web = 40,
        LowPriority = 98,
        MediumPriority = 99,
        HighPriority = 100,
    }

    [CreateAssetMenu(menuName = "Avrahamy/Setup/Debug", fileName = "DebugLog")]
    public class DebugLog : ScriptableObject
    {
        [Serializable]
        public struct LogTagAndColor
        {
            public bool show;
            public LogTag tag;
            public Color color;
        }

        private static DateTime LogStartTime
        {
            get
            {
                if (!hasStarted)
                {
                    hasStarted = true;
                    _startTime = DateTime.Now;
                }

                return _startTime;
            }
        }

        private static DateTime _startTime;
        private static bool hasStarted;

        [SerializeField]
        bool _logEverything = true;

        [SerializeField]
        LogTagAndColor[] _activeTags;

        private static bool logEverything = true;
        private static LogTagAndColor[] activeTags;

        private static readonly Color DefaultLogColor = Color.white;
        private static readonly Color DefaultWarningColor = Color.yellow;
        private static readonly Color DefaultErrorColor = Color.red;

        protected void Awake()
        {
            OnEnable();
        }

        protected void OnEnable()
        {
            logEverything = _logEverything;
            activeTags = _activeTags;
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        #region Log

        [Conditional("DEBUG_LOG")]
        public static void Log(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.Log(GetLogWithColorAndTime(message, DefaultLogColor), context);
        }

        [Conditional("DEBUG_LOG")]
        internal static void LogIf(bool condition, object message, UnityEngine.Object context = null)
        {
            if (condition)
                Log(GetLogWithColorAndTime(message, DefaultLogColor), context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogWarning(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogWarning(GetLogWithColorAndTime(message, DefaultWarningColor), context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogError(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogError(GetLogWithColorAndTime(message, DefaultErrorColor), context);
        }
        
        // TODO: add log exception, assertion and all the rest.

        #endregion // Log

        #region Log with Color

        [Conditional("DEBUG_LOG")]
        public static void Log(object message, Color color, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.Log(GetLogWithColorAndTime(message, color), context);
        }

        [Conditional("DEBUG_LOG")]
        internal static void LogIf(bool condition, object message, Color color,
            UnityEngine.Object context = null)
        {
            if (condition)
            {
                Log(GetLogWithColorAndTime(message, color), context);
            }
        }

        [Conditional("DEBUG_LOG")]
        public static void LogWarning(object message, Color color, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogWarning(GetLogWithColorAndTime(message, color), context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogError(object message, Color color, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogError(GetLogWithColorAndTime(message, color), context);
        }

        #endregion // Log with Color

        #region Log with Tag

        [Conditional("DEBUG_LOG")]
        public static void Log(LogTag tag, object message, UnityEngine.Object context = null)
        {
            if (!ShouldLog(tag))
                return;
            UnityEngine.Debug.Log(GetLogWithColorAndTime(message, DefaultLogColor), context);
        }

        [Conditional("DEBUG_LOG")]
        internal static void LogIf(LogTag tag, bool condition, object message,
            UnityEngine.Object context = null)
        {
            if (condition)
                Log(tag, GetLogWithColorAndTime(message, DefaultLogColor), context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogWarning(LogTag tag, object message, UnityEngine.Object context = null)
        {
            if (!ShouldLog(tag))
                return;
            UnityEngine.Debug.LogWarning(GetLogWithColorAndTime(tag, message), context);
        }

        [Conditional("DEBUG_LOG")]
        public static void LogError(LogTag tag, object message, UnityEngine.Object context = null)
        {
            if (!ShouldLog(tag))
                return;
            UnityEngine.Debug.LogError(GetLogWithColorAndTime(tag, message), context);
        }

        private static bool ShouldLog(LogTag tag)
        {
#if UNITY_EDITOR
            // Hack to get the DebugLog to always load.
            if (activeTags == null)
            {
                var filter = "t:DebugLog";
                var guids = UnityEditor.AssetDatabase.FindAssets(filter);
                if (guids.Length == 0)
                    return true;
                var guid = guids[0];
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<DebugLog>(path);
                if (asset == null)
                    return true;
                asset.OnEnable();
            }
#endif
            if (logEverything || activeTags == null) return true;
            foreach (var activeTag in activeTags)
            {
                if (tag == activeTag.tag) return activeTag.show;
            }

            return false;
        }

        private static Color GetLogColor(LogTag tag)
        {
#if UNITY_EDITOR
            if (activeTags == null) return Color.white;
            foreach (var activeTag in activeTags)
            {
                if (tag == activeTag.tag) return activeTag.color;
            }
#endif
            return Color.white;
        }

        private static object GetLogWithColorAndTime(LogTag tag, object message)
        {
            var color = GetLogColor(tag);
            message = GetLogWithColorAndTime(message, color);
#if UNITY_EDITOR
            if (tag == LogTag.LowPriority)
            {
                // Add a linebreak so it will remove the first stack trace line from the console.
                message += "\n";
            }
#endif
            return message;
        }

        private static object GetLogWithColorAndTime(object message, Color color)
        {
            if (color != Color.white)
            {
                message = $"{Time.realtimeSinceStartup:0.00} - <color=#{color.ToHex()}>{message}</color>";
            }
            else
            {
                message = $"{Time.realtimeSinceStartup:0.00} - {message}";
            }

            return message;
        }

        #endregion // Log with Tag

        #region Log on Thread

        /// <summary>
        /// This is needed if we want to log messages from a thread that is not the
        /// main thread (for example, callbacks from native code).
        /// </summary>
        [Conditional("DEBUG_LOG")]
        public static void LogOnThread(object message)
        {
            TimeSpan span = DateTime.Now - LogStartTime;
            UnityEngine.Debug.Log(span.TotalSeconds.ToString("0.00") + ": " + message);
        }

        /// <summary>
        /// This is needed if we want to log messages from a thread that is not the
        /// main thread (for example, callbacks from native code).
        /// </summary>
        [Conditional("DEBUG_LOG")]
        public static void LogErrorOnThread(object message)
        {
            TimeSpan span = DateTime.Now - LogStartTime;
            UnityEngine.Debug.LogError(span.TotalSeconds.ToString("0.00") + ": " + message);
        }

        #endregion // Log on Thread

    }
}
