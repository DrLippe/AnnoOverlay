using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AnnoOverlay.Helpers
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static void ReadPointerPath(Process process, int[] offsets, ref short result)
        {
            long address = (long)process.MainModule.BaseAddress;
            int pHandle = (int)process.Handle;

            byte[] buffer = new byte[8];
            int bytesRead = 0;

            foreach (int offset in offsets)
            {
                ReadProcessMemory(pHandle, address + offset, buffer, 8, ref bytesRead);
                address = BitConverter.ToInt64(buffer, 0);
            }

            result = BitConverter.ToInt16(buffer, 0);
        }

        public static void ReadPointerPath(Process process, int[] offsets, ref int result)
        {
            long address = (long)process.MainModule.BaseAddress;
            int pHandle = (int)process.Handle;

            byte[] buffer = new byte[8];
            int bytesRead = 0;

            foreach (int offset in offsets)
            {
                ReadProcessMemory(pHandle, address + offset, buffer, 8, ref bytesRead);
                address = BitConverter.ToInt64(buffer, 0);
            }

            result = BitConverter.ToInt32(buffer, 0);
        }

        public static void ReadPointerPath(Process process, int[] offsets, ref long result)
        {
            long address = (long)process.MainModule.BaseAddress;
            int pHandle = (int)process.Handle;

            byte[] buffer = new byte[8];
            int bytesRead = 0;

            foreach (int offset in offsets)
            {
                ReadProcessMemory(pHandle, address + offset, buffer, 8, ref bytesRead);
                address = BitConverter.ToInt64(buffer, 0);
            }

            result = BitConverter.ToInt64(buffer, 0);
        }

        public static void ReadPointerPath(Process process, int[] offsets, ref byte[] result)
        {
            long address = (long)process.MainModule.BaseAddress;
            int pHandle = (int)process.Handle;

            byte[] buffer = new byte[8];
            int bytesRead = 0;

            foreach (int offset in offsets)
            {
                if (offset == offsets.Last())
                {
                    ReadProcessMemory(pHandle, address + offset, result, result.Length, ref bytesRead);
                }
                else
                {
                    ReadProcessMemory(pHandle, address + offset, buffer, 8, ref bytesRead);
                    address = BitConverter.ToInt64(buffer, 0);
                }
            }
        }

        // Hotkey
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardListener.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        public static IntPtr SetWindowsHook(int idHook, LowLevelKeyboardListener.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId)
        {
            return SetWindowsHookEx(idHook, lpfn, hMod, dwThreadId);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        public static bool UnhookWindowsHook(IntPtr hhk)
        {
            return UnhookWindowsHookEx(hhk);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        public static IntPtr CallNextHook(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(hhk, nCode, wParam, lParam);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        public static IntPtr GetCurrentModuleHandle(string lpModuleName)
        {
            return GetModuleHandle(lpModuleName);
        }
    }
}
