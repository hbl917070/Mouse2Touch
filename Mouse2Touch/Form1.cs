using Gma.UserActivityMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Mouse2Touch {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }


        public static bool bool_stop = false;//焦點的視窗在排除名單裡面，暫時停用觸控功能
        public static bool bool_m_handled = true;//按右鍵時，攔截原本的訊號
        public static Mtype mtype = Mtype.滑鼠側鍵;//操作類型

        private System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();//右下角圖示

        bool bool_exe_run = true;//程式是否在運行中
        bool bool_down = false;

        bool bool_down_b1 = false;//按下側鍵
        bool bool_down_ml = false;//按下滑鼠左鍵
        bool bool_down_mr = false;//按下滑鼠右鍵
        bool bool_down_mm = false;//按下滑鼠中鍵

        bool bool_down_mr_moved = false;//按下右鍵，並且已經觸發移動
        float f_touch_speed = 1;//觸控速度

        List<String> ar_ad = new List<String>();//排除名單

        //模擬觸控用
        Point pp = new Point();//上一次移動的位置(判斷移動距離)
        Point pp_start = new Point();//按下的位置
        PointerTouchInfo contact;
        PointerFlags oFlags;



        /// <summary>
        /// 操作類型
        /// </summary>
        public enum Mtype {
            //stop = -1,
            none = 0,
            滑鼠側鍵 = 1,
            滑鼠左鍵 = 2,
            滑鼠右鍵 = 3, //長按右鍵，開啟右鍵選單
            滑鼠右鍵_2 = 4, //原地按右鍵，開啟右鍵選單
            滑鼠中鍵 = 5,
            鍵盤 = 6
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender2, EventArgs e2) {


            String html_path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "main_ui.html");
            if (File.Exists(html_path)) {

                webBrowser1.Navigate(html_path);
                webBrowser1.ObjectForScripting = new C_js_to_net(this);//讓網頁允許存取C#
                webBrowser1.AllowNavigation = false;//禁止載入其他網頁
                //webBrowser1.IsWebBrowserContextMenuEnabled = false;//禁止右鍵

            } else {
                webBrowser1.DocumentText = $"Can't find 「{html_path}」";
            }

            TouchInjector.InitializeTouchInjection();//初始化觸控API


            //
            var tim = new System.Windows.Forms.Timer();
            tim.Interval = 200;
            tim.Tick += (sender, e) => {

                String gn = GetWindowsName.GetName();//取得目前焦點視窗的名稱

                //與「排除名單」進行比對
                foreach (var item in ar_ad) {
                    if (gn.IndexOf(item, StringComparison.CurrentCultureIgnoreCase) > -1) {
                        bool_stop = true;
                        return;
                    }
                }
                bool_stop = false;
            };
            tim.Start();



            new Thread(() => {


                while (bool_exe_run) {

                    if (mtype == Mtype.滑鼠右鍵_2) {

                        bool bool_d = bool_down_mr;

                        // Touch Down Simulate
                        if ((bool_down == false) && bool_d) {
                            bool_down = true;
                            pp = getPos();
                            pp_start = pp;
                            bool_down_mr_moved = false;
                        }

                        // Touch Up Simulate
                        if (bool_down && (bool_d == false)) {
                            bool_down = false;

                            if (bool_down_mr_moved) {
                                //模擬觸控-結束
                                contact.PointerInfo.PointerFlags = PointerFlags.UP;
                                TouchInjector.InjectTouchInput(1, new[] { contact });

                            } else {

                                RightClick();
                            }
                            bool_down_mr_moved = false;
                            //upFull();
                        } else

                        // Touch Move Simulate
                        if (bool_down) {
                            var p2 = getPos();

                            if (bool_down_mr_moved) {

                                //模擬觸控-移動
                                int nMoveIntervalX = (int)((p2.X - pp.X) * f_touch_speed);
                                int nMoveIntervalY = (int)((p2.Y - pp.Y) * f_touch_speed);
                                contact.Move(nMoveIntervalX, nMoveIntervalY);
                                oFlags = PointerFlags.INRANGE | PointerFlags.INCONTACT | PointerFlags.UPDATE;
                                contact.PointerInfo.PointerFlags = oFlags;
                                TouchInjector.InjectTouchInput(1, new[] { contact });

                                //讓滑鼠跟隨觸控的位置
                                if (f_touch_speed != 1) {
                                    NativeMethods.SetCursorPos(pp.X + nMoveIntervalX, pp.Y + nMoveIntervalY);
                                    p2 = getPos();
                                }

                                pp = p2;

                                if (overd_sc(pp)) {//超出螢幕時自動停止觸控
                                    upFull();
                                }

                            } else {

                                if (func_位移(pp_start, p2)) {
                                    bool_down_mr_moved = true;

                                    //模擬觸控-開始
                                    contact = MakePointerTouchInfo(pp_start.X, pp_start.Y);
                                    oFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;
                                    contact.PointerInfo.PointerFlags = oFlags;
                                    bool bIsSuccess = TouchInjector.InjectTouchInput(1, new[] { contact });
                                }

                            }

                        }

                    } else {

                        bool bool_d = bool_down_b1 || bool_down_ml || bool_down_mr || bool_down_mm;

                        // Touch Down Simulate
                        if ((bool_down == false) && bool_d) {
                            bool_down = true;
                            pp = getPos();

                            if (mtype == Mtype.滑鼠右鍵) {
                                bool_m_handled = false;
                            }

                            contact = MakePointerTouchInfo(pp.X, pp.Y);
                            oFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;
                            contact.PointerInfo.PointerFlags = oFlags;
                            bool bIsSuccess = TouchInjector.InjectTouchInput(1, new[] { contact });

                            if (mtype == Mtype.滑鼠右鍵) {
                                Thread.Sleep(1);
                                bool_m_handled = true;
                            }
                        }

                        // Touch Up Simulate
                        if (bool_down && (bool_d == false)) {
                            bool_down = false;

                            if (mtype == Mtype.滑鼠右鍵) { bool_m_handled = false; }

                            contact.PointerInfo.PointerFlags = PointerFlags.UP;
                            TouchInjector.InjectTouchInput(1, new[] { contact });

                            if (mtype == Mtype.滑鼠右鍵) {
                                Thread.Sleep(1);
                                bool_m_handled = true;
                            }
                        }

                        // Touch Move Simulate
                        if (bool_down) {

                            var p2 = getPos();
                            int nMoveIntervalX = (int)((p2.X - pp.X) * f_touch_speed);
                            int nMoveIntervalY = (int)((p2.Y - pp.Y) * f_touch_speed);

                            contact.Move(nMoveIntervalX, nMoveIntervalY);
                            oFlags = PointerFlags.INRANGE | PointerFlags.INCONTACT | PointerFlags.UPDATE;
                            contact.PointerInfo.PointerFlags = oFlags;
                            TouchInjector.InjectTouchInput(1, new[] { contact });

                            //讓滑鼠跟隨觸控的位置
                            if (f_touch_speed != 1) {
                                NativeMethods.SetCursorPos(pp.X + nMoveIntervalX, pp.Y + nMoveIntervalY);
                                p2 = getPos();
                            }

                            pp = p2;
                            if (overd_sc(pp)) {//超出螢幕時自動停止觸控
                                upFull();
                            }


                        }

                    }


                    Thread.Sleep(11);

                }

            }).Start();


            HookManager.MouseDown += HookManager_MouseDown;
            HookManager.MouseUp += HookManager_MouseUp;
            //HookManager.MouseMove += HookManager_MouseMove;

            this.FormClosing += Form1_FormClosing;
            init_nIcon();


        }



        /// <summary>
        /// 關閉程式時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            bool_exe_run = false;
            HookManager.MouseDown -= HookManager_MouseDown;
            HookManager.MouseUp -= HookManager_MouseUp;
        }



        /// <summary>
        /// 判斷螢幕觸控點是否碰到螢幕邊緣
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool overd_sc(Point p) {

            var sc = Screen.GetWorkingArea(p);
            if (p.X <= sc.X || p.Y <= sc.Y || (p.X >= sc.X + sc.Width) || (p.Y >= sc.Y + sc.Height)) {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 設定觸控速度
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public String set_touch_speed(String t) {
            try {
                f_touch_speed = float.Parse(t);
                if (f_touch_speed < 1) { f_touch_speed = 1f; }
                if (f_touch_speed > 5) { f_touch_speed = 5; }
                //System.Console.WriteLine(t);
                return "true";
            } catch (Exception) {
                f_touch_speed = 1;
                return "false";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public String set_Mtype(String t) {
            if (t == "none") { mtype = Mtype.none; return "true"; }
            if (t == "m_b1") { mtype = Mtype.滑鼠側鍵; return "true"; }
            if (t == "m_l") { mtype = Mtype.滑鼠左鍵; return "true"; }
            if (t == "m_mm") { mtype = Mtype.滑鼠中鍵; return "true"; }
            if (t == "m_r") { mtype = Mtype.滑鼠右鍵; return "true"; }
            if (t == "m_r_2") { mtype = Mtype.滑鼠右鍵_2; return "true"; }

            return "false";
        }


        /// <summary>
        /// 設定排除名單
        /// </summary>
        /// <param name="s"></param>
        public void set_ad(String s) {


            if (s == null) { s = ""; }
            s = s.Trim();
            var ar = s.Split('\n');

            ar_ad = new List<string>();

            foreach (var item in ar) {
                String s2 = item.Trim();
                if (s2.Length > 0 && ar_ad.Contains(s2) == false) {
                    ar_ad.Add(s2);
                }
            }

        }


        /// <summary>
        /// 取得 config
        /// </summary>
        /// <returns></returns>
        public String get_config() {
            String json_path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.json");
            String s = "";

            if (File.Exists(json_path)) {
                using (StreamReader sr = new StreamReader(json_path, Encoding.UTF8)) {
                    s = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return s;
        }

        /// <summary>
        /// 設定 config
        /// </summary>
        public void set_config(String s) {
            String json_path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.json");

            using (FileStream fs = new FileStream(json_path, FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                    sw.WriteLine(s);
                }
            }

        }





        /// <summary>
        /// 判斷滑鼠是否有移動
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        bool func_位移(Point p1, Point p2) {
            int x = Math.Abs(p1.X) - Math.Abs(p2.X);
            int y = Math.Abs(p1.Y) - Math.Abs(p2.Y);
            bool b = (Math.Abs(x) + Math.Abs(y) >= 5);//如果兩點超過5像素就判斷有位移
            return b;
        }




        /// <summary>
        /// 按下滑鼠的按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HookManager_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {

            if (bool_stop) {//強制終止所有操作
                return;
            }

            if (mtype == Mtype.滑鼠側鍵 && e.Button == MouseButtons.XButton1) {
                bool_down_b1 = true;
                return;
            }

            if (mtype == Mtype.滑鼠左鍵 && e.Button == MouseButtons.Left) {
                bool_down_ml = true;
                return;
            }

            if (mtype == Mtype.滑鼠右鍵 && e.Button == MouseButtons.Right) {
                bool_down_mr = true;
                return;
            }

            if (mtype == Mtype.滑鼠右鍵_2 && e.Button == MouseButtons.Right) {
                bool_down_mr = true;
                return;
            }

            if (mtype == Mtype.滑鼠中鍵 && e.Button == MouseButtons.Middle) {
                bool_down_mm = true;
                return;
            }
        }

        /// <summary>
        /// 放開滑鼠的按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HookManager_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
            upFull();
        }



        /// <summary>
        /// 釋放按鍵時呼叫
        /// </summary>
        private void upFull() {
            bool_down_b1 = false;
            bool_down_ml = false;
            bool_down_mr = false;
            bool_down_mm = false;
            bool_m_handled = true;//按右鍵時，攔截原本的訊號
        }



        /// <summary>
        /// 模擬點擊滑鼠右鍵。開啟右鍵選單
        /// </summary>
        private void RightClick() {
            var mt = mtype;
            mtype = Mtype.none;
            NativeMethods.RightDown();
            NativeMethods.RightUp();
            Thread.Sleep(1);
            mtype = mt;
        }



        /// <summary>
        /// 初始化。右下角的圖示
        /// </summary>
        public void init_nIcon() {

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            nIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            nIcon.Text = "Mouse2Touch";
            nIcon.Visible = false;//隱藏右下角圖示

            var cm = new System.Windows.Forms.ContextMenu();
            cm.MenuItems.Add("Show", new EventHandler((sender2, e2) => {
                this.Visible = true;//顯示視窗
                nIcon.Visible = false;//隱藏右下角圖示
            }));

            cm.MenuItems.Add("Close", new EventHandler((sender2, e2) => {
                this.Close();//關閉程式
            }));

            nIcon.ContextMenu = cm;

            nIcon.DoubleClick += (sender, e) => {
                this.Visible = true;//顯示視窗
                nIcon.Visible = false;//隱藏右下角圖示
            };

        }


        /// <summary>
        /// 將程式縮小至右下角
        /// </summary>
        public void hide_window() {
            this.Visible = false;
            nIcon.Visible = true;//顯示右下角圖示
        }



        /// <summary>
        /// 取得目前滑鼠坐標
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Point getPos() {

            return System.Windows.Forms.Cursor.Position;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public void print(String s) {
            System.Console.WriteLine(s);
        }



        /// <summary>
        /// 模擬觸控移動
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointerTouchInfo MakePointerTouchInfo(int x, int y) {

            uint orientation = 87;//角度
            uint pressure = 256;//壓力
            int radius = 1;//觸控半徑

            PointerTouchInfo contact = new PointerTouchInfo();
            contact.PointerInfo.pointerType = PointerInputType.TOUCH;
            contact.TouchFlags = TouchFlags.NONE;
            contact.Orientation = orientation;
            contact.Pressure = pressure;
            contact.TouchMasks = TouchMask.CONTACTAREA | TouchMask.ORIENTATION | TouchMask.PRESSURE;
            contact.PointerInfo.PtPixelLocation.X = x;
            contact.PointerInfo.PtPixelLocation.Y = y;
            uint unPointerId = IdGenerator.GetUinqueUInt();
            //Console.WriteLine("PointerId    " + unPointerId);
            contact.PointerInfo.PointerId = unPointerId;
            contact.ContactArea.left = x - radius;
            contact.ContactArea.right = x + radius;
            contact.ContactArea.top = y - radius;
            contact.ContactArea.bottom = y + radius;
            return contact;
        }



    }





    [ComVisible(true)]
    public class C_js_to_net {

        Form1 M;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        public C_js_to_net(Form1 f) {
            this.M = f;
        }

        /// <summary>
        /// 設定觸控速度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public String set_touch_speed(String s) {
            return M.set_touch_speed(s);
        }

        /// <summary>
        /// 設定操作類型
        /// </summary>
        /// <param name="s"></param>
        public void set_Mtype(String s) {
            M.set_Mtype(s);
        }

        /// <summary>
        /// 隱藏視窗
        /// </summary>
        public void hide_window() {
            M.hide_window();
        }

        /// <summary>
        /// 設定排除名單
        /// </summary>
        /// <param name="s"></param>
        public void set_ad(String s) {
            M.set_ad(s);
        }

        /// <summary>
        /// 讀取設定檔
        /// </summary>
        /// <returns></returns>
        public String get_config() {
            return M.get_config();
        }

        /// <summary>
        /// 儲存設定檔
        /// </summary>
        /// <param name="s"></param>
        public void set_config(String s) {
            M.set_config(s);
        }

        /// <summary>
        /// 開啟網址
        /// </summary>
        /// <param name="url"></param>
        public void open_url(String url) {
            //呼叫系統預設的瀏覽器 
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 列印文字，用於debug
        /// </summary>
        /// <param name="s"></param>
        public void print(String s) {
            M.print(s);
        }
    }

}
