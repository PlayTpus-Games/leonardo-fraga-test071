using UnityEngine;

public static class ArrayExtensions
{
    /// <summary>
    /// Fisher-Yates Shuffling (Time: O(n), Space O(1))
    /// </summary>
    /// <returns>Shuffled arrays</returns>
    public static T[] Shuffle<T>(this T[] array)
    {
        int r = array.Length - 1;
        while (r > 0)
        {
            int l = Random.Range(0, r);
            (array[l], array[r]) = (array[r], array[l]);
            r--;
        }

        return array;
    }
}
