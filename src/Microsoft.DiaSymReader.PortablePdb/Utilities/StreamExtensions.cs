﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the License.txt file in the project root for more information.

using System.Diagnostics;
using System.IO;

namespace Microsoft.DiaSymReader.PortablePdb
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Attempts to read all of the requested bytes from the stream into the buffer
        /// </summary>
        /// <returns>
        /// The number of bytes read. Less than <paramref name="count" /> will
        /// only be returned if the end of stream is reached before all bytes can be read.
        /// </returns>
        /// <remarks>
        /// Unlike <see cref="Stream.Read(byte[], int, int)"/> it is not guaranteed that
        /// the stream position or the output buffer will be unchanged if an exception is
        /// returned.
        /// </remarks>
        internal static int TryReadAll(this Stream stream, byte[] buffer, int offset, int count)
        {
            // The implementations for many streams, e.g. FileStream, allows 0 bytes to be
            // read and returns 0, but the documentation for Stream.Read states that 0 is
            // only returned when the end of the stream has been reached. Rather than deal
            // with this contradiction, let's just never pass a count of 0 bytes
            Debug.Assert(count > 0);

            int totalBytesRead;
            int bytesRead = 0;
            for (totalBytesRead = 0; totalBytesRead < count; totalBytesRead += bytesRead)
            {
                // Note: Don't attempt to save state in-between calls to .Read as it would
                // require a possibly massive intermediate buffer array
                bytesRead = stream.Read(buffer,
                                        offset + totalBytesRead,
                                        count - totalBytesRead);
                if (bytesRead == 0)
                {
                    break;
                }
            }
            return totalBytesRead;
        }
    }
}
