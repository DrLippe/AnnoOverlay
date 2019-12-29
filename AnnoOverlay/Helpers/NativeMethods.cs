using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AnnoOverlay.Helpers
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

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
    }
}
