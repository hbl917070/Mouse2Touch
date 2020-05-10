using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mouse2Touch {

    /// <summary>
    /// 限制滑鼠在特定區域
    /// </summary>
    /*class ClipCursorHelper {
    
        [DllImport("user32.dll")]
        static extern bool ClipCursor(ref RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        extern static int GetWindowRect(int hwnd, ref RECT lpRect);

        /// <summary>
        /// 
        /// </summary>
        public struct RECT {

            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(Int32 left, Int32 top, Int32 right, Int32 bottom) {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

        }

        /// <summary>
        /// 設置鼠標顯示在主屏范圍內
        /// </summary>
        /// <returns></returns>
        public static bool SetCursorInPrimaryScreen() {

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens.OrderBy(t => t.WorkingArea.X).First();
            //RECT rect = new RECT(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Right + screen.Bounds.X, screen.Bounds.Bottom);
            RECT rect = new RECT(300,300,401,401);
            return ClipCursor(ref rect);
        }


        /// <summary>
        /// 恢復鼠標顯示，可以所以屏幕的任何位置
        /// </summary>
        /// <returns></returns>
        public static bool Default() {

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens.OrderByDescending(t => t.WorkingArea.X).First();
            RECT rect = new RECT(0, 0, screen.Bounds.Right + screen.Bounds.X, screen.Bounds.Bottom);

            return ClipCursor(ref rect);
        }



    }*/
}
