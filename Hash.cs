using System;
using System.Text;

public static class Hash
{
    private static readonly uint[] InitialState =
    {
        0x67452301,
        0xEFCDAB89,
        0x98BADCFE,
        0x10325476,
        0xC3D2E1F0
    };

    public static string ToSHA1(string input)
    {
        byte[] data = Encoding.UTF8.GetBytes(input);
        byte[] paddedData = PadMessage(data);

        uint[] H = (uint[])InitialState.Clone();

        for (int i = 0; i < paddedData.Length; i += 64)
        {
            uint[] w = new uint[80];

            for (int j = 0; j < 16; j++)
            {
                w[j] =
                    ((uint)paddedData[i + j * 4] << 24) |
                    ((uint)paddedData[i + j * 4 + 1] << 16) |
                    ((uint)paddedData[i + j * 4 + 2] << 8) |
                    ((uint)paddedData[i + j * 4 + 3]);
            }

            for (int j = 16; j < 80; j++)
            {
                w[j] = RotateLeft(w[j - 3] ^ w[j - 8] ^ w[j - 14] ^ w[j - 16], 1);
            }

            uint a = H[0];
            uint b = H[1];
            uint c = H[2];
            uint d = H[3];
            uint e = H[4];

            for (int j = 0; j < 80; j++)
            {
                uint f, k;

                if (j < 20)
                {
                    f = (b & c) | ((~b) & d);
                    k = 0x5A827999;
                }
                else if (j < 40)
                {
                    f = b ^ c ^ d;
                    k = 0x6ED9EBA1;
                }
                else if (j < 60)
                {
                    f = (b & c) | (b & d) | (c & d);
                    k = 0x8F1BBCDC;
                }
                else
                {
                    f = b ^ c ^ d;
                    k = 0xCA62C1D6;
                }

                uint temp = RotateLeft(a, 5) + f + e + k + w[j];
                e = d;
                d = c;
                c = RotateLeft(b, 30);
                b = a;
                a = temp;
            }

            H[0] += a;
            H[1] += b;
            H[2] += c;
            H[3] += d;
            H[4] += e;
        }

        return $"{H[0]:x8}{H[1]:x8}{H[2]:x8}{H[3]:x8}{H[4]:x8}";
    }

    private static byte[] PadMessage(byte[] data)
    {
        ulong bitLength = (ulong)data.Length * 8;
        int paddingLength = (56 - (data.Length + 1) % 64 + 64) % 64;
        
        if (paddingLength < 0)
            paddingLength += 64;

        byte[] padded = new byte[data.Length + 1 + paddingLength + 8];

        Array.Copy(data, padded, data.Length);

        padded[data.Length] = 0x80;

        for (int i = 0; i < 8; i++)
        {
            padded[padded.Length - 1 - i] = (byte)(bitLength >> (8 * i));
        }

        return padded;
    }

    private static uint RotateLeft(uint value, int bits)
    {
        return (value << bits) | (value >> (32 - bits));
    }
}