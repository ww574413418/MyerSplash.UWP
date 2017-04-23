using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.Common
{
    public static class NavigationService
    {
        private static Frame RootFrame
        {
            get
            {
                return Window.Current.Content as Frame;
            }
        }

        private static Stack<Func<bool>> HistoryOperations { get; set; } = new Stack<Func<bool>>();

        public static void AddOperation(Func<bool> func)
        {
            HistoryOperations.Push(func);
        }

        public static bool GoBack()
        {
            var handled = false;

            while (!handled)
            {
                if (HistoryOperations.Count > 0)
                {
                    var op = HistoryOperations.Pop();
                    handled = op.Invoke();
                }
                else break;
            }

            return handled;
        }
    }
}
