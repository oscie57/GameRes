using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameRes
{

    public class Taskbar
    {
        [DllImport("USER32.DLL")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("USER32.DLL")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        [DllImport("USER32.DLL")]
        public static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("USER32.DLL")]

        public static extern bool IsWindowVisible(int hWnd);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
            public RECT rcDevice;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        protected static int Handle
        {
            get
            {
                return FindWindow("Shell_TrayWnd", "");
            }
        }
        public static bool CheckShown()
        {
            return IsWindowVisible(Handle);
        }
        private Taskbar()
        {
            // hide ctor
        }

        public static void Show()
        {
            ShowWindow(Handle, SW_SHOW);
        }

        public static void Hide()
        {
            ShowWindow(Handle, SW_HIDE);
        }
    }

}
