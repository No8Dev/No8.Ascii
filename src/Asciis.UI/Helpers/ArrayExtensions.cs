namespace Asciis.UI;

public static class ArrayExtensions
{
    public static T? RemoveLast<T>(this List<T>? list)
    {
        if (list == null || list.Count <= 0)
            return default;

        var value = list[^1];
        list.RemoveAt(list.Count - 1);

        return value;
    }

    public static T RemoveItem<T>(this List<T> list, int index)
    {
        var value = list[index];
        list.RemoveAt(index);

        return value;
    }

    public static int IndexOf(this char[] array, char item)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] == item) return i;

        return -1;
    }

    public static void Fill<T>(this T[] originalArray, T with)
    {
        for (int i = 0; i < originalArray.Length; i++)
            originalArray[i] = with;
    }

    public static void Fill<T>(this T[] originalArray, Func<int, T> action)
    {
        for (int i = 0; i < originalArray.Length; i++)
            originalArray[i] = action(i);
    }

}
