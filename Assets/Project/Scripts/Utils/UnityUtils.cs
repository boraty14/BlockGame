using System.Collections.Generic;
using System;

namespace Project.Scripts.Utils
{
    public static class UnityUtils
    {
        private static readonly Random Rng = new Random();  
        public static void ShuffleList<T>(this IList<T> shuffleList)
        {
            int n = shuffleList.Count;  
            while (n > 1) {  
                n--;  
                int k = Rng.Next(n + 1);  
                (shuffleList[k], shuffleList[n]) = (shuffleList[n], shuffleList[k]);
            }  
        }
    }
}
