using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Spookbox
{
    internal class Logger
    {
        public static void Log(string info)
        {
            Debug.Log($"[Spookbox]: {info}");
        }
        public static void LogError(string error)
        {
            Debug.LogError($"[Spookbox]: {error}");
        }

        public static void LogWarning(string warning)
        {
            Debug.LogWarning($"[Spookbox]: {warning}");
        }
    }
}
