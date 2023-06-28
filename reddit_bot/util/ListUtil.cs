using System;
using System.Collections.Generic;

namespace reddit_bor.util
{
    internal class ListUtil
    {
        public static void Shuffle<T>(List<T> list)
        {
            Random random = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
