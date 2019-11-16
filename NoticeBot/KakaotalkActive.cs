using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoticeBot
{
    class KakaotalkActive
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        public IntPtr kakaohd;
        public IntPtr mainhd;
        public IntPtr chathd;
        public IntPtr freindsrhd;
        public IntPtr resulthd;
        public IntPtr talkhd;

        public static IntPtr SetPrevious(IntPtr handle, string classname, int time)
        {
            var previous = IntPtr.Zero;
            var nowhandle = FindWindowEx(handle, previous, classname, null);
            for (int i = 0; i < time; i++)
            {
                previous = nowhandle;
                nowhandle = FindWindowEx(handle, previous, classname, null);
            }

            return nowhandle;

        }

        public KakaotalkActive()
        {
            kakaohd = FindWindow("EVA_Window_Dblclk", "카카오톡");
            mainhd = FindWindowEx(kakaohd, IntPtr.Zero, "EVA_ChildWindow", null);
            chathd = SetPrevious(mainhd, "EVA_Window", 1);
            freindsrhd = SetPrevious(chathd, "Edit", 0);
            resulthd = SetPrevious(chathd, "EVA_VH_ListControl_Dblclk", 1);
        }

        public void KatalkSend(string name, string context, bool image, string file)
        {
            SendMessage(freindsrhd, 0x000c, IntPtr.Zero, name);
            Thread.Sleep(500);
            NativeHelper.LbuttonDoubleClick(resulthd, 0, 0);
            Thread.Sleep(300);
            talkhd = FindWindow("#32770", null);
            IntPtr talktexthd = FindWindowEx(talkhd, IntPtr.Zero, "RichEdit20W", "");
            SendMessage(talktexthd, 0x000c, IntPtr.Zero, context);
            PostMessage(talktexthd, 0x0100, 0xD, 0x1C001);
            if (image == true)
            {
                Thread.Sleep(100);
                NativeHelper.LbuttonClick(talkhd, 89, 582);
                Thread.Sleep(300);
                IntPtr filehd = FindWindow("#32770", "열기");
                IntPtr filenamehd = FindWindowEx(filehd, IntPtr.Zero, "ComboBoxEx32", "");
                Thread.Sleep(300);
                SendMessage(filenamehd, 0x000c, IntPtr.Zero, file);
                Thread.Sleep(200);
                PostMessage(filehd, 0x0100, 0xD, 0x1C001);
                Thread.Sleep(300);
            }
            Thread.Sleep(200);
            KakaotalkActive.SendMessage(talkhd, 0x0010, IntPtr.Zero, "0");
        }
    }
}
