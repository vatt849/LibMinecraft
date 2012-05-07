using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft
{
    public class RC4 // Credit for this code goes to Delaney Gillilan, who has released it into the public domain.
    {
        #region Private Stream State
        private byte[] S = new byte[256];       // The key stream state
        private uint BaseI, BaseJ;              // The indexers into the key
        #endregion

        #region Static Members
        // Swap the values in the given key stream
        private static void swap(byte[] S, uint x, uint y)
        {
            byte temp = S[x];
            S[x] = S[y];
            S[y] = temp;
        }

        // Get the RC4 output to be XORed with the input
        private static byte rc4_output(byte[] S, ref uint i, ref uint j)
        {
            i = (i + 1) & 255;
            j = (j + S[i]) & 255;

            swap(S, i, j);

            return S[(S[i] + S[j]) & 255];
        }
        #endregion

        #region Constructors
        // Initialize the keystream
        private void Init(byte[] Key, uint DropCount)
        {
            // Init to defaults: every entry gets the value of its index
            for (BaseI = 0; BaseI < 256; BaseI++)
                S[BaseI] = (byte)BaseI;

            // Swizzle the data based on the key
            for (BaseI = BaseJ = 0; BaseI < 256; BaseI++)
            {
                BaseJ = (BaseJ + Key[(int)(BaseI % Key.Length)] + S[BaseI]) & 255;
                swap(S, BaseI, BaseJ);
            }

            // Reset the indexers
            BaseI = BaseJ = 0;

            // Drop the first N bytes of the sequence
            for (uint k = 0; k < DropCount; k++)
            {
                rc4_output(S, ref BaseI, ref BaseJ);
            }
        }

        public RC4(string Key, uint DropCount)
        {
            Init(System.Text.Encoding.ASCII.GetBytes(Key), DropCount);
        }

        public RC4(string Key)
        {
            Init(System.Text.Encoding.ASCII.GetBytes(Key), 768);
        }

        public RC4(string Key, System.Text.Encoding Encoding, uint DropCount)
        {
            Init(Encoding.GetBytes(Key), DropCount);
        }

        public RC4(string Key, System.Text.Encoding Encoding)
        {
            Init(Encoding.GetBytes(Key), 768);
        }

        public RC4(byte[] Key, uint DropCount)
        {
            Init(Key, DropCount);
        }

        public RC4(byte[] Key)
        {
            Init(Key, 768);
        }
        #endregion

        #region Generic Crypt Function
        // Encrypt or decrypt the byte array
        private byte[] Crypt(byte[] Input)
        {
            // First, create a copy of the base stream state
            byte[] s = new byte[S.Length];
            S.CopyTo(s, 0);
            uint i = BaseI, j = BaseJ;

            byte[] ReturnValue = new byte[Input.Length];

            // XOR each entry in the data with its RC4 byte
            int Index = 0;
            foreach (byte b in Input)
            {
                ReturnValue[Index++] = (byte)(b ^ rc4_output(s, ref i, ref j));
            }

            return ReturnValue;
        }
        #endregion

        #region Encryption
        // Encrypt the string using ASCII encoding
        public byte[] Encrypt(string Input)
        {
            return Encrypt(Input, System.Text.Encoding.ASCII);
        }

        // Encrypt the string using the given encoding
        public byte[] Encrypt(string Input, System.Text.Encoding Encoding)
        {
            return Crypt(Encoding.GetBytes(Input));
        }

        // Encrypt the given byte array
        public byte[] Encrypt(byte[] Input)
        {
            return Crypt(Input);
        }
        #endregion

        #region Decryption
        // Decrypt the data into a string using ASCII encoding
        public string DecryptString(byte[] Input)
        {
            return DecryptString(Input, System.Text.Encoding.ASCII);
        }

        // Decrypt the data into a string using the given encoding
        public string DecryptString(byte[] Input, System.Text.Encoding Encoding)
        {
            return Encoding.GetString(Crypt(Input));
        }

        // Decrypt the given byte array
        public byte[] Decrypt(byte[] Input)
        {
            return Crypt(Input);
        }
        #endregion
    }
}
