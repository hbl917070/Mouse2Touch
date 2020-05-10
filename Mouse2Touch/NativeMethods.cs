using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mouse2Touch {

    /// <summary>
    /// 滑鼠模擬
    /// </summary>
    class NativeMethods {


        #region #####Mouse#####
        [STAThread]
        [DllImport("User32")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);


        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        [DllImport("User32")]
        public extern static bool GetCursorPos(out Point p);
        [DllImport("User32")]
        public extern static int ShowCursor(bool bShow);
        #endregion



        #region 滑鼠模擬

        /// <summary>
        /// 模擬壓住滑鼠左鍵。
        /// </summary>
        public static void LeftDown() {
            NativeMethods.mouse_event(NativeContansts.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬釋放滑鼠左鍵。
        /// </summary>
        public static void LeftUp() {
            NativeMethods.mouse_event(NativeContansts.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬點擊滑鼠左鍵。
        /// </summary>
        public static void LeftClick() {
            LeftDown();
            LeftUp();
        }

        /// <summary>
        /// 模擬壓住滑鼠右鍵。
        /// </summary>
        public static void RightDown() {
            NativeMethods.mouse_event(NativeContansts.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬釋放滑鼠右鍵。
        /// </summary>
        public static void RightUp() {
            NativeMethods.mouse_event(NativeContansts.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬點擊滑鼠右鍵。
        /// </summary>
        public static void RightClick() {
            RightDown();
            RightUp();
        }

        /// <summary>
        /// 模擬壓住滑鼠中鍵。
        /// </summary>
        public static void MiddleDown() {
            NativeMethods.mouse_event(NativeContansts.MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬釋放滑鼠中鍵。
        /// </summary>
        public static void MiddleUp() {
            NativeMethods.mouse_event(NativeContansts.MOUSEEVENTF_MIDDLEUP, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬點擊滑鼠中鍵。
        /// </summary>
        public static void MiddleClick() {
            MiddleDown();
            MiddleUp();
        }

        #endregion



    }


    public class NativeContansts {

        //滑鼠左鍵
        public static int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public static int MOUSEEVENTF_LEFTUP = 0x0004;

        //滑鼠右鍵
        public static int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public static int MOUSEEVENTF_RIGHTUP = 0x0010;

        //滑鼠中鍵
        public static int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public static int MOUSEEVENTF_MIDDLEUP = 0x0040;

        //滑鼠側鍵
        public static int MOUSEEVENTF_XDOWN = 0x0080;
        public static int MOUSEEVENTF_XUP = 0x0100;

        //滾輪
        public static int MOUSEEVENTF_WHEEL = 0x0800;
        public static int MOUSEEVENTF_HWHEEL = 0x01000;


    }

}
