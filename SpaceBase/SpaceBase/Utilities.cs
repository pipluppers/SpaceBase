using System.Windows.Documents;

namespace SpaceBase
{
    interface ISerializable
    {
        string Serialize();
        object? Deserialize(string str);
    }

    #region Enumerations

    /// <summary>
    /// Represents what kind of card this is.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// A standard ship card belonging to sectors 1-12 and having a level.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Similar to a standard card but possessing a charge cube effect.
        /// </summary>
        Charge = 1,

        /// <summary>
        /// A card that has no activation effect.
        /// </summary>
        Colony = 2
    }

    /// <summary>
    /// Represents when the charge cube effect can be activated.
    /// </summary>
    public enum ChargeCardType
    {
        /// <summary>
        /// Can only be activated on the player's turn. In the physical game, the color is blue.
        /// </summary>
        Turn = 0,

        /// <summary>
        /// Can only be activated on an opponent's turn. In the physical game, the color is red.
        /// </summary>
        OpponentTurn = 1,

        /// <summary>
        /// Can only be activated on any player's turn. In the physical game, the color is green.
        /// </summary>
        Anytime = 2
    }

    #endregion Enumerations

    internal static class Utilities
    {
        /// <summary>
        /// Gets the first ancestor of the given type.
        /// </summary>
        /// <typeparam name="T">The type for the ancestor.</typeparam>
        /// <param name="current">The child of the ancestor element to find.</param>
        /// <returns>The first ancestor of the given type.</returns>
        public static T? FindAncestor<T>(DependencyObject current) where T : class
        {
            return FindAncestor<T>(current, 1);
        }

        /// <summary>
        /// Gets the n-th ancestor of the given type.
        /// </summary>
        /// <typeparam name="T">The type for the ancestor.</typeparam>
        /// <param name="current">The child of the ancestor element to find.</param>
        /// <param name="ancestorNumber">The nth ancestor to find.</param>
        /// <returns>The first ancestor of the given type.</returns>
        public static T? FindAncestor<T>(DependencyObject current, int ancestorNumber) where T : class
        {
            while (current != null)
            {
                if (current is T ancestor)
                {
                    if (ancestorNumber == 1)
                        return ancestor;
                    else
                        --ancestorNumber;
                }

                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        /// <summary>
        /// Gets the first visual child of the given type.
        /// </summary>
        /// <typeparam name="T">The type for the child.</typeparam>
        /// <param name="parent">The parent of the child element to find.</param>
        /// <returns>The first visual child of the given type.</returns>
        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }

                T? descendant = FindVisualChild<T>(child);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the n-th visual child at the first level of the given type.
        /// </summary>
        /// <typeparam name="T">The type for the child.</typeparam>
        /// <param name="parent">The parent of the child element to find.</param>
        /// <param name="childNumber">The nth ancestor to find.</param>
        /// <returns>The first visual child of the given type.</returns>
        public static T? FindVisualChild<T>(DependencyObject parent, int childNumber) where T : DependencyObject
        {
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            int childCounter = childNumber;

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    if (childCounter == 1)
                        return typedChild;
                    else
                        --childCounter;
                }

                for (int j = i + 1; j < count; ++j)
                {
                    if (child is T typedChild2)
                    {
                        if (childCounter == 1)
                            return typedChild2;
                        else
                            --childCounter;
                    }
                }

                T? descendant = FindVisualChild<T>(child, childNumber);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the string representation of the serializable object.
        /// </summary>
        /// <param name="serializableObject">The object to get the string representation of.</param>
        /// <returns>The string representation of the given object.</returns>
        public static string Serialize(ISerializable serializableObject) => serializableObject.Serialize();

        /// <summary>
        /// Remove all adorners from a UI element.
        /// </summary>
        /// <param name="element">The element to remove all adorners from.</param>
        public static void RemoveAllAdorners(UIElement element)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null) return;

            var adorners = adornerLayer.GetAdorners(element);
            if (adorners == null) return;

            foreach (var adorner in adorners)
                adornerLayer.Remove(adorner);
        }
    }

}
