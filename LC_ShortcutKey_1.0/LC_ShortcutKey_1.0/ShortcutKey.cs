using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using Limbuskeyboardshortcuts;

namespace LC_ShortcutKey_1._0
{
    internal class ShortcutKey : IDisposable
    {
        private bool disposedValue = false;

        private void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    Timer.Dispose();
                }
                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        ~ShortcutKey()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            Timer = new MultimediaTimer() { Interval = 7, Resolution = 5 };
            Timer.Elapsed += MoveHandler;
            Timer.Start();
            SetHook();
        }

        public void Toggle_PlayState()
        {
            if (IsStop)
            {
                Resume();
            }
            else
            {
                Stop();
            }

            IsStop = !IsStop;
        }

        public void Stop()
        {
            IsStop = true;
        }

        public void Resume()
        {
            IsStop = false;
        }

        public void End()
        {
            Timer.Stop();
            Dispose();
            UnHook();
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private static IntPtr hookId = IntPtr.Zero;

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        LowLevelKeyboardProc _hproc = HookProc;

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hproc, hInstance, 0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(hookId);
        }

        private static bool IsStop = false;

        private static IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (IsStop) return CallNextHookEx(hookId, nCode, (int)wParam, lParam);

                if (wParam == (IntPtr)WM_KEYDOWN) //일반 키 눌림
                {
                    Keys key = (Keys)Marshal.ReadInt32(lParam);
                    switch (key)
                    {
                        case Keys.Right:
                            rightpressed = true;
                            return (IntPtr)1;

                        case Keys.Left:
                            leftpressed = true;
                            return (IntPtr)1;

                        case Keys.Down:
                            backpressed= true;
                            return (IntPtr)1;

                        case Keys.Up:
                            fowardpressed = true;
                            return (IntPtr)1;

                        case Keys.S:
                            Click_Skip();
                            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);

                        case Keys.X:
                            WinRateFight();
                            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);

                        case Keys.C:
                            DamageAmountFigt();
                            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);

                        case Keys.Z:
                            mouse_event(LBUTTONDOWN, 0, 0, 0, 0);
                            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);

                        case Keys.Space:
                            Click_Eixt();
                            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);
                    }
                }
                else if (wParam == (IntPtr)WM_KEYUP)
                {
                    Keys key = (Keys)Marshal.ReadInt32(lParam);
                    switch (key) 
                    {
                        case Keys.Right:
                            rightpressed = false;
                            return (IntPtr)1;
                        case Keys.Left:
                            leftpressed = false;
                            return (IntPtr)1;
                        case Keys.Down:
                            backpressed = false;
                            return (IntPtr)1;
                        case Keys.Up:
                            fowardpressed = false;
                            return (IntPtr)1;
                        case Keys.Z:
                            mouse_event(LBUTTONUP, 0, 0, 0, 0);
                            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);
                    }
                }
            }
            return CallNextHookEx(hookId, nCode, (int)wParam, lParam);
        }

        protected const uint LBUTTONDOWN = 0x0002;    // 왼쪽 마우스 버튼 누름
        protected const uint LBUTTONUP = 0x0004;      // 왼쪽 마우스 버튼 땜
        protected const uint MOUSEEVENTF_MOVE = 0x0001;
        protected const uint MOUSEEVENTF_WHEEL = 0x0800;
        protected const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("user32.dll")]
        protected static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        protected static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

        private static int ABSOLUTE_X(int X)
        {
            return X * 65535 / Screen.PrimaryScreen.Bounds.Width;
        }

        private static int ABSOLUTE_Y(int Y)
        {
            return Y * 65535 / Screen.PrimaryScreen.Bounds.Height;
        }

        private static void WinRateFight()
        {
            keybd_event((byte)Keys.P, 0, 0x00, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.P, 0, 0x02, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.Enter, 0, 0x00, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.Enter, 0, 0x02, 0);
        }

        private static void DamageAmountFigt()
        {
            keybd_event((byte)Keys.P, 0, 0x00, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.P, 0, 0x02, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.P, 0, 0x00, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.P, 0, 0x02, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.Enter, 0, 0x00, 0);
            Thread.Sleep(20);
            keybd_event((byte)Keys.Enter, 0, 0x02, 0);
        }

        private static bool fowardpressed = false;
        private static bool backpressed = false;
        private static bool rightpressed = false;
        private static bool leftpressed = false;

        private int dx = 0;
        private int dy = 0;
        private MultimediaTimer Timer;

        private void MoveHandler(object sender, EventArgs e)
        {
            if (!IsStop)
            {
                dx = 0;
                dy = 0;
                if (fowardpressed || backpressed || leftpressed || rightpressed)
                {
                    Speed++;
                }
                else
                {
                    Speed--;
                }

                if (fowardpressed) dy -= 1;
                if (backpressed) dy += 1;
                if (leftpressed) dx -= 1;
                if (rightpressed) dx += 1;
                mouse_event(MOUSEEVENTF_MOVE, dx * Speed, dy * Speed, 0, 0);
            }
        }

        private static int speed;
        private static int Speed
        {
            set
            {
                if ((speed + value < 30) && (speed + value > 0))
                {
                    speed = value;
                }
            }

            get { return speed; }
        }

        private static void Click_Skip()
        {
            Rectangle rect = Screen.PrimaryScreen.Bounds;
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            }

            int checkpixel = 0;
            float rate;
            
            Rectangle skip_rect = new Rectangle(new Point(861, 473), new Size(5, 5));
            //checkskip
            Color SkipColor = bmp.GetPixel(skip_rect.X, skip_rect.Y);
            for (int x1 = skip_rect.X; x1 < skip_rect.X + skip_rect.Width + 1; x1++)
            {
                for (int y1 = skip_rect.Y; y1 < skip_rect.Y + skip_rect.Height + 1; y1++)
                {
                    Color color = bmp.GetPixel(x1, y1);
                    if (SkipColor == color)
                    {
                        checkpixel++;
                    }
                }
            }
            rate = checkpixel / 25 * 100;

            if (rate >= 80)
            {
                //clickskip
                Point cursorPosition = Cursor.Position;
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, ABSOLUTE_X(skip_rect.X), ABSOLUTE_Y(skip_rect.Y), 0, 0);
                mouse_event(LBUTTONDOWN, 0, 0, 0, 0);
                Thread.Sleep(50);
                mouse_event(LBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(50);
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, ABSOLUTE_X(cursorPosition.X), ABSOLUTE_Y(cursorPosition.Y), 0, 0);
            }
        }

        private static void Click_Eixt()
        {
            Rectangle rect = Screen.PrimaryScreen.Bounds;
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            // Bitmap 이미지 변경을 위해 Graphics 객체 생성
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            }

            Rectangle eixt_rect = new Rectangle(new Point(1578, 931) ,new Size(5, 5));

            //skipcheck
            int checkpixel = 0;
            float rate;

            //checkeixt
            for (int x2 = eixt_rect.X; x2 < eixt_rect.X + eixt_rect.Width + 1; x2++)
            {
                for (int y2 = eixt_rect.Y; y2 < eixt_rect.Y + eixt_rect.Height + 1; y2++)
                {
                    Color color = bmp.GetPixel(x2, y2);
                    if (color.R >= 160)
                    {
                        checkpixel++;
                    }
                }
            }
            rate = checkpixel / 25 * 100;

            if (rate >= 80)
            {
                //clickEixt
                Point cursorPosition = Cursor.Position;
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, ABSOLUTE_X(eixt_rect.X), ABSOLUTE_Y(eixt_rect.Y), 0, 0);
                mouse_event(LBUTTONDOWN, 0, 0, 0, 0);
                Thread.Sleep(50);
                mouse_event(LBUTTONUP, 0, 0, 0, 0);
                Thread.Sleep(50);
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, ABSOLUTE_X(cursorPosition.X), ABSOLUTE_Y(cursorPosition.Y), 0, 0);
                return;
            }
        }
    }
}
