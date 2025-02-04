﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTask
{
    public static class KBEmulator
    {
        public enum InputType : uint
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 3
        }

        [Flags]
        internal enum KEYEVENTF : uint
        {
            KEYDOWN = 0x0,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        [Flags]
        internal enum MOUSEEVENTF : uint
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }

        // Master Input structure
        [StructLayout(LayoutKind.Sequential)]
        public struct lpInput
        {
            internal InputType type;
            internal InputUnion Data;
            internal static int Size { get { return Marshal.SizeOf(typeof(lpInput)); } }
        }

        // Union structure
        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        // Input Types
        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal long dx;
            internal long dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal short wVk;
            internal short wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        private class unmanaged
        {
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint SendInput(
                uint cInputs,
                [MarshalAs(UnmanagedType.LPArray)]
            lpInput[] inputs,
                int cbSize
            );

            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern short VkKeyScan(char ch);

            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int GetSystemMetrics(int nIndex);
        }

        internal static short VkKeyScan(char ch)
        {
            return unmanaged.VkKeyScan(ch);
        }

        internal static int GetSystemMetrics(int nIndex)
        {
            return unmanaged.GetSystemMetrics(nIndex);
        }

        internal static uint SendInput(uint cInputs, lpInput[] inputs, int cbSize)
        {
            return unmanaged.SendInput(cInputs, inputs, cbSize);
        }

        public static void SendScanCode(short scanCode)
        {
            lpInput[] KeyInputs = new lpInput[1];
            lpInput KeyInput = new lpInput();
            // Generic Keyboard Event
            KeyInput.type = InputType.INPUT_KEYBOARD;
            KeyInput.Data.ki.wScan = 0;
            KeyInput.Data.ki.time = 0;
            KeyInput.Data.ki.dwExtraInfo = UIntPtr.Zero;


            // Push the correct key
            KeyInput.Data.ki.wVk = scanCode;
            KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYDOWN;
            KeyInputs[0] = KeyInput;
            SendInput(1, KeyInputs, lpInput.Size);

            // Release the key
            KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYUP;
            KeyInputs[0] = KeyInput;
            SendInput(1, KeyInputs, lpInput.Size);

            return;
        }

        /*
        public static void SendKeyboard(char ch)
        {
            lpInput[] KeyInputs = new lpInput[1];
            lpInput KeyInput = new lpInput();
            // Generic Keyboard Event
            KeyInput.type = InputType.INPUT_KEYBOARD;
            KeyInput.Data.ki.wScan = 0;
            KeyInput.Data.ki.time = 0;
            KeyInput.Data.ki.dwExtraInfo = UIntPtr.Zero;


            // Push the correct key
            KeyInput.Data.ki.wVk = VkKeyScan(ch);
            KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYDOWN;
            KeyInputs[0] = KeyInput;
            SendInput(1, KeyInputs, lpInput.Size);

            // Release the key
            KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYUP;
            KeyInputs[0] = KeyInput;
            SendInput(1, KeyInputs, lpInput.Size);

            return;
        }*/
        public static void SendKeyboard(char ch)
        {
            lpInput[] KeyInputs = new lpInput[1];
            lpInput KeyInput = new lpInput();
            // Generic Keyboard Event
            KeyInput.type = InputType.INPUT_KEYBOARD;
            KeyInput.Data.ki.wScan = 0;
            KeyInput.Data.ki.time = 10;
            KeyInput.Data.ki.dwExtraInfo = UIntPtr.Zero;


            // Push the correct key
            KeyInput.Data.ki.wVk = VkKeyScan(ch);
            KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYDOWN;
            KeyInput.Data.ki.time = 10;
            KeyInputs[0] = KeyInput;
            SendInput(1, KeyInputs, 2);

            // Release the key
            KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYUP;
            KeyInput.Data.ki.time = 0;
            KeyInputs[0] = KeyInput;
            SendInput(1, KeyInputs, 2);

            return;
        }

        // Attempt at overloading for multiple key presses
        public static void SendKeyboard(char[] ch)
        {
            lpInput[] KeyInputs = new lpInput[ch.Length];

            // Push the correct key
            for (int i = 0; i < ch.Length; i++)
            {
                // Generate new memory address for KeyInput each time
                lpInput KeyInput = new lpInput();
                // Generic Keyboard Event
                KeyInput.type = InputType.INPUT_KEYBOARD;
                KeyInput.Data.ki.wScan = 0;
                KeyInput.Data.ki.time = 0;
                KeyInput.Data.ki.dwExtraInfo = UIntPtr.Zero;
                KeyInput.Data.ki.wVk = VkKeyScan(ch[i]);
                KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYDOWN;
                KeyInputs[i] = KeyInput;
            }
            SendInput(Convert.ToUInt32(ch.Length), KeyInputs, (lpInput.Size * ch.Length));

            // Release the key
            for (int i = 0; i < ch.Length; i++)
            {
                // Generate new memory address for KeyInput each time
                lpInput KeyInput = new lpInput();
                // Generic Keyboard Event
                KeyInput.type = InputType.INPUT_KEYBOARD;
                KeyInput.Data.ki.wScan = 0;
                KeyInput.Data.ki.time = 0;
                KeyInput.Data.ki.dwExtraInfo = UIntPtr.Zero;
                KeyInput.Data.ki.wVk = VkKeyScan(ch[i]);
                KeyInput.Data.ki.dwFlags = KEYEVENTF.KEYUP;
                KeyInputs[i] = KeyInput;
            }
            SendInput(Convert.ToUInt32(ch.Length), KeyInputs, (lpInput.Size * ch.Length));

            return;
        }


        public static void SendMouseClick(long dx, long dy)
        {
            lpInput[] MouseInputs = new lpInput[1];
            lpInput MouseClickInput = new lpInput();
            // Generic Keyboard Event
            MouseClickInput.type = InputType.INPUT_MOUSE;

            MouseClickInput.Data.mi.dwFlags = MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE;
            MouseClickInput.Data.mi.dx = dx * (65535 / GetSystemMetrics(0)); //x being coord in pixels
            MouseClickInput.Data.mi.dy = dy * (65535 / GetSystemMetrics(1)); //y being coord in pixels
            SendInput(1, MouseInputs, lpInput.Size);


            //MouseClickInput.Data.mi.time = 0;
            MouseClickInput.Data.mi.dwExtraInfo = UIntPtr.Zero;


            MouseClickInput.Data.mi.dwFlags = MOUSEEVENTF.LEFTDOWN;
            MouseInputs[0] = MouseClickInput;
            SendInput(1, MouseInputs, lpInput.Size);

            
            //MouseClickInput.Data.ki.wVk = VkKeyScan(ch);
            //MouseClickInput.Data.ki.dwFlags = KEYEVENTF.KEYDOWN;
            //KeyInputs[0] = MouseClickInput;
            //SendInput(1, KeyInputs, lpInput.Size);
            //
            //// Release the key
            MouseClickInput.Data.mi.dwFlags = MOUSEEVENTF.LEFTUP;
            MouseInputs[0] = MouseClickInput;
            SendInput(1, MouseInputs, lpInput.Size);

            return;
        }


        public static void ClickLeftMouseButton()
        {
            lpInput[] MouseInputs = new lpInput[1];

            lpInput mouseDownInput = new lpInput();
            mouseDownInput.type = InputType.INPUT_MOUSE;
            mouseDownInput.Data.mi.dwFlags = MOUSEEVENTF.LEFTDOWN;
            MouseInputs[0] = mouseDownInput;
            SendInput(1, MouseInputs, lpInput.Size);


            lpInput mouseUpInput = new lpInput();
            mouseUpInput.type = InputType.INPUT_MOUSE;
            mouseUpInput.Data.mi.dwFlags = MOUSEEVENTF.LEFTUP;
            MouseInputs[0] = mouseUpInput;
            SendInput(1, MouseInputs, lpInput.Size);
        }
        public static void ClickRightMouseButton()
        {
            lpInput[] MouseInputs = new lpInput[1];

            lpInput mouseDownInput = new lpInput();
            mouseDownInput.type = InputType.INPUT_MOUSE;
            mouseDownInput.Data.mi.dwFlags = MOUSEEVENTF.RIGHTDOWN;
            MouseInputs[0] = mouseDownInput;
            SendInput(1, MouseInputs, lpInput.Size);



            lpInput mouseUpInput = new lpInput();
            mouseUpInput.type = InputType.INPUT_MOUSE;
            mouseUpInput.Data.mi.dwFlags = MOUSEEVENTF.RIGHTUP;
            MouseInputs[0] = mouseUpInput;
            SendInput(1, MouseInputs, lpInput.Size);
        }



        public static void SetMousePosition(int x, int y, int width, int height)
        {
            lpInput[] MouseInputs = new lpInput[1];

            lpInput mouseMoveInput = new lpInput();
            mouseMoveInput.type = InputType.INPUT_MOUSE;



            mouseMoveInput.Data.mi.dwFlags = MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE;



            mouseMoveInput.Data.mi.dx = 65535 * x / width;
            mouseMoveInput.Data.mi.dy = 65535 * y / height;



            MouseInputs[0] = mouseMoveInput;
            SendInput(1, MouseInputs, lpInput.Size);
        }




    }
}
