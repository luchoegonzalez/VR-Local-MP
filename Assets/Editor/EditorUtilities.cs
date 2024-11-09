using System.Linq;
using UnityEditor;
public static class EditorUtilities
{

    /// <summary>
    /// Checks for duplicates in the given values.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="values">The values to check for duplicates.</param>
    /// <param name="getValueFunc">The function to get the value to check for duplicates.</param>
    /// <param name="objectName">The name of the object to display in the warning message.</param>
    public static void CheckForDuplicateOrders<T>(T[] values, System.Func<T, string> getValueFunc, string objectName)
    {
        if (values == null || values.Length == 0) return;

        var duplicateOrders = values
            .GroupBy(getValueFunc)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateOrders.Any())
        {
            foreach (var order in duplicateOrders)
            {
                EditorGUILayout.HelpBox($"Duplicate {objectName} order found: {order}", MessageType.Warning);
            }
        }
    }
}
