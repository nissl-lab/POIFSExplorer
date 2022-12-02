// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2022, Jiaqi Liu. All rights reserved.
//  Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// ---------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;

namespace Cpk.Net
{
    internal static class Utility
    {
        internal static async Task<T> ReadStruct<T>(FileStream stream) where T : struct
        {
            var structSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[structSize];
            await stream.ReadAsync(buffer,0, structSize);
            return ByteArrayToStruct<T>(buffer);
        }

        private static unsafe T ByteArrayToStruct<T>(byte[] bytes) where T : struct
        {
            fixed (byte* ptr = &bytes[0])
            {
                return (T) (Marshal.PtrToStructure((IntPtr) ptr, typeof(T)) ?? default(T));
            }
        }

        private static int GetPatternIndex(byte[] src, byte[] pattern, int startIndex = 0)
        {
            var maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (var i = startIndex; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0])
                    continue;

                for (var j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }

            return -1;
        }

        public static byte[] TrimEnd(byte[] buffer, byte[] pattern)
        {
            var length = GetPatternIndex(buffer, pattern);
            return length == -1 ? buffer : new ArraySegment<byte>(buffer, 0, length).ToArray();
        }
    }
}