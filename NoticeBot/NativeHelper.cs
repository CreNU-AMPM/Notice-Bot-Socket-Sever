using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public static class NativeHelper
{
    public static IntPtr MakeLParam(int LoWord, int HiWord)
    {
        return (IntPtr)((int)((HiWord << 16) | (LoWord & 0xFFFF)));
    }
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const int WM_SETTEXT = 0x000C;
    public const uint WM_MOUSEACTIVATE = 0x0021;
    public const uint WM_LBUTTONUP = 0x0202;

    public const uint WM_MOUSEWHEEL = 0x020A;
    public const int WM_VSCROLL2 = 277;
    public const uint WM_KEYDOWN = 0x0100;
    public const int VK_DOWN = 0x28;
    public const uint WM_KEYUP = 0x0101;
    public const uint WM_CHAR = 0x0102;
    public const int WM_SETCURSOR = 0x0020;
    public const int WM_MOUSEMOVE = 0x0200;
    public const int VK_TAB = 0x09;
    public const int VK_ENTER = 0x0D;

    public const uint WM_LBUTTONDBLCLK = 0x0203;
    public enum HitTestCodes
    {
        HTCLIENT = 1
    }
    public static void LbuttonDoubleClick(IntPtr handle, int x, int y)
    {
        NativeHelper.PostMessage(handle, NativeHelper.WM_LBUTTONDBLCLK, (IntPtr)1, NativeHelper.MakeLParam(x, y));
        Thread.Sleep(33);
        NativeHelper.PostMessage(handle, NativeHelper.WM_LBUTTONUP, (IntPtr)0, NativeHelper.MakeLParam(x, y));
    }
    public static void LbuttonClick(IntPtr handle, int x, int y)
    {
        NativeHelper.PostMessage(handle, NativeHelper.WM_LBUTTONDOWN, (IntPtr)1, NativeHelper.MakeLParam(x, y));
        Thread.Sleep(33);
        NativeHelper.PostMessage(handle, NativeHelper.WM_LBUTTONUP, (IntPtr)0, NativeHelper.MakeLParam(x, y));
    }
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, string lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int GetWindowTextLength(IntPtr hWnd);
    public static string GetWindowTitle(IntPtr hWnd)
    {
        var sb = new StringBuilder(GetWindowTextLength(hWnd) + 1);
        int length = GetWindowText(hWnd, sb, sb.Capacity);
        if (length > 0)
        { return Convert.ToString(sb); }
        return string.Empty;
    }
    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
    public static extern bool DeleteDC([In] IntPtr hdc);
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
    [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject([In] IntPtr hObject);
    public static Rectangle GetWindowRectangle(IntPtr hwnd)
    {
        var rect = new RECT();
        if (GetWindowRect(hwnd, out rect))
        { return new Rectangle(rect.Location, rect.Size); }
        return Rectangle.Empty;
    }
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);
    public enum SetWindowPosFlags : uint
    {
        SynchronousWindowPosition = 0x4000,
        DeferErase = 0x2000,
        DrawFrame = 0x0020,
        FrameChanged = 0x0020,
        HideWindow = 0x0080,
        DoNotActivate = 0x0010,
        DoNotCopyBits = 0x0100,
        IgnoreMove = 0x0002,
        DoNotChangeOwnerZOrder = 0x0200,
        DoNotRedraw = 0x0008,
        DoNotReposition = 0x0200,
        DoNotSendChangingEvent = 0x0400,
        IgnoreResize = 0x0001,
        IgnoreZOrder = 0x0004,
        ShowWindow = 0x0040,
    }
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
    public static Bitmap GetWindowScreen(IntPtr hWnd)
    {
        var wdc = GetWindowDC(hWnd);
        var cdc = CreateCompatibleDC(wdc);

        var rect = GetWindowRectangle(hWnd);

        var bitmap = CreateCompatibleBitmap(wdc, rect.Width, rect.Height);
        var temp = SelectObject(cdc, bitmap);

        BitBlt(cdc, 0, 0, rect.Width, rect.Height, wdc, 0, 0, 0x00CC0020);

        SelectObject(cdc, temp);

        DeleteDC(cdc);
        ReleaseDC(hWnd, wdc);


        Bitmap bmp = Image.FromHbitmap(bitmap);

        DeleteObject(bitmap);

        return bmp;
    }

    public static string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size > 0)
        {
            var builder = new StringBuilder(size + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        return String.Empty;
    }
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetFocus(IntPtr hWnd);
    public static uint BM_SETCHECK = 0x00f1;
    public static int BST_CHECKED = 0x0001;
    static Color GetPixel(int X, int Y, Bitmap b)
    {
        //  Console.WriteLine(X + " / " + Y + " : " + b.GetPixel(X, Y).ToString());
        return b.GetPixel(X, Y);
    }
    public static Point ColorPicker(Bitmap b, Color c)
    {
        for (int x = 0; x < b.Width; x++)
        {
            for (int y = 0; y < b.Height; y++)
            {
                if (GetPixel(x, y, b) == c)
                {
                    return new Point(x, y);
                }
            }
        }
        return new Point(0, 0);
    }
    public static bool ColorPickerb(Bitmap b, Color c)
    {
        for (int x = 0; x < b.Width; x++)
        {
            for (int y = 0; y < b.Height; y++)
            {
                if (GetPixel(x, y, b) == c)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static string hexget(int a)
    {
        return string.Format("{0:X8}", a);
    }
    public static string GetClassNameOfWindow(IntPtr hwnd)
    {
        string className = "";
        StringBuilder classText = null;
        try
        {
            int cls_max_length = 1000;
            classText = new StringBuilder("", cls_max_length + 5);
            GetClassName(hwnd, classText, cls_max_length + 2);

            if (!String.IsNullOrEmpty(classText.ToString()) && !String.IsNullOrWhiteSpace(classText.ToString()))
                className = classText.ToString();
        }
        catch (Exception ex)
        {
            className = ex.Message;
        }
        finally
        {
            classText = null;
        }
        return className;
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

}

public static class Extensions
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    public static IEnumerable<IntPtr> Children(this IntPtr self, string Class = null, string Title = null)
    {
        var previous = IntPtr.Zero;
        do
        {
            var next = FindWindowEx(self, previous, Class, Title);
            if (next.Equals(IntPtr.Zero) == false)
            {
                yield return next;
            }
            previous = next;
        }
        while (previous.Equals(IntPtr.Zero) == false);
    }
}

