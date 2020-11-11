using System;
using System.Collections.Generic;
using System.Text;

namespace WebSockets.Common
{
    internal static class WebSocketFrameCommon
    {
        public const int MaskKeyLength = 4;

        /// <summary>
        /// Mutate payload with the mask key
        /// This is a reversible process
        /// If you apply this to maked data it will be unmasked and visa versa
        /// </summary>
        /// <param name="maskKey">The 4 byte mask key</param>
        /// <param name="payload">The payload to mutate</param>
        public static void ToggleMask(byte[] maskKey, byte[] payload)
        {
            if (maskKey.Length != MaskKeyLength)
            {
                throw new Exception($"MaskKey key must be {MaskKeyLength} bytes");
            }

            // apply the mask key (this is a reversible process so no need to copy the payload)
            for (int i = 0; i < payload.Length; i++)
            {
                payload[i] = (Byte)(payload[i] ^ maskKey[i % MaskKeyLength]);
            }
        }
    }
}
