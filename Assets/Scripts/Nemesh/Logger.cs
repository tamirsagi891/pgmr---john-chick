using System;
using Avrahamy.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nemesh
{
    public static class Logger  // TODO: use ILogger and ILogHandler!
    {
        public static readonly Color DontFormat = Color.clear;
        private static readonly Color DefaultLogColor = Color.white;
        private static readonly Color DefaultExceptionColor = Color.red;
        private static readonly Color DefaultWarningColor = Color.yellow;
        private static readonly Color DefaultAssertionColor = Color.magenta;

        private static string FormattedMessage(object message, Color color)
        {
            // Get the current time
            float currentTime = Time.realtimeSinceStartup;

            string formattedMessage;
            if (color.Equals(DontFormat))
            {
                formattedMessage = $"{currentTime:0.00} - {message}";
            }
            else
            {
                formattedMessage = $"{currentTime:0.00} - <color=#{color.ToHex()}>{message}</color>";
            }

            return formattedMessage;
        }

        public static void Log(object message, Color color, Object context = null)
        {
            var formattedMessage = FormattedMessage(message, color);

            // Write the message to the log, using the context object as the context
            UnityEngine.Debug.Log(formattedMessage, context);
        }

        public static void Log(object message, Object context = null)
        {
            // Use the default color for Log()
            Log(message, DefaultLogColor, context);
        }

        public static void LogException(object message, Color color, Object context = null)
        {
            var formattedMessage = FormattedMessage(message, color);

            // Write the message as an exception to the log, using the context object as the context
            UnityEngine.Debug.LogException(new Exception(formattedMessage), context);
        }

        public static void LogException(object message, Object context = null)
        {
            // Use the default color for LogException()
            LogException(message, DefaultExceptionColor, context);
        }

        public static void LogWarning(object message, Color color, Object context = null)
        {
            var formattedMessage = FormattedMessage(message, color);

            // Write the message as a warning to the log, using the context object as the context
            UnityEngine.Debug.LogWarning(formattedMessage, context);
        }

        public static void LogWarning(object message, Object context = null)
        {
            // Use the default color for LogWarning()
            LogWarning(message, DefaultWarningColor, context);
        }

        public static void LogAssertion(object message, Color color, Object context = null)
        {
            var formattedMessage = FormattedMessage(message, color);

            // Write the message to the log, using the context object as the context
            UnityEngine.Debug.LogAssertion(formattedMessage, context);
        }

        public static void LogAssertion(object message, Object context = null)
        {
            // Use the default color for LogAssertion()
            LogAssertion(message, DefaultAssertionColor, context);
        }

        // public static void LogAssertionFormat(Object context, string format, params object[] args)
        // {
        //     Debug.LogAssertionFormat(context, );
        // }
    }
}