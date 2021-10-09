using System;
using System.Runtime.InteropServices;

class Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
    public static extern bool GetCursorPos(out POINT pt);

    [DllImport("user32.dll", EntryPoint = "GetKeyState")]
    public static extern int GetKeyState(int nVirtKey);

    [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
    public static extern IntPtr WindowFromPoint(POINT Point);

    [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
    public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

    public static int GWL_EXSTYLE = -20;
    public static uint WS_EX_TRANSPARENT = 0x00000020;
}
