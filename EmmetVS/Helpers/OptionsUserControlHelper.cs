using System.Windows.Media;
using System.Windows;

namespace EmmetVS.Helpers;

/// <summary>
/// Represent the options user control helper.
/// </summary>
public static class OptionsUserControlHelper
{
    /// <summary>
    /// Finds child
    /// </summary>
    /// <typeparam name="T">Type parameter</typeparam>
    /// <param name="parent">Parent dependency object</param>
    /// <param name="childIndex">Child index</param>
    /// <returns>An object</returns>
    public static T FindChild<T>(DependencyObject parent, int childIndex) where T : DependencyObject
    {
        if (parent == null) return null;

        T childElement = null;
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is not T)
            {
                childElement = FindChild<T>(child, childIndex);  // Recursive call
                if (childElement != null) break;
            }
            else if (i == childIndex)
            {
                childElement = (T)child;
                break;
            }
        }
        return childElement;
    }
}
