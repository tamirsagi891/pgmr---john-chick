using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class CorotuineUtils
    {
        public static IEnumerator DelayExecution(float delay, Action method, Func<bool> predicate)
        {
            yield return new WaitForSeconds(delay);
            if (predicate())
            {
                method();
            }
        }

        public static IEnumerator DelayExecution(float delay, Action method)
        {
            yield return new WaitForSeconds(delay);
            method();
        }
    }
}
