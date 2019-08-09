using System;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace GetHash
{
    class Program
    {
        static byte[] FILE_DATA;

        static MD5 md5 = MD5.Create();
        static SHA1 sha1 = SHA1.Create();
        static SHA256 sha256 = SHA256.Create();
        static SHA384 sha384 = SHA384.Create();
        static SHA512 sha512 = SHA512.Create();
        static RIPEMD160 ripemd160 = RIPEMD160.Create();

        static void Main(string[] args)
        {
            string[] dialog = {
                "MD5       - ",
                "RIPEMD160 - ",
                "SHA1      - ",
                "SHA256    - ",
                "SHA384    - ",
                "SHA512    - "
            };
            Console.WriteLine("GetHash version 1.0.1 Copyright (C) Josh Stephenson. All rights reserved.");

            Console.Write("File to hash: ");
            byte[][] hashTable = doWork(Console.ReadLine());
            Console.WriteLine();
            for (int i = 0; i < 6; ++i) {
                Console.Write(dialog[i]);
                Console.WriteLine(BitConverter.ToString(hashTable[i]));
            }
        }

        static byte[][] doWork(string filePath)
        {
            byte[][] retValue = new byte[6][];
            FileInfo fi = null;

            try { fi = new FileInfo(filePath); }
            catch (Exception ex){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR - " + ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(0xFF);
            }

            if (fi.Length <= 1610612736){

                FILE_DATA = File.ReadAllBytes(filePath);

                Task[] taskArray = new Task[6]
                {
                    Task.Factory.StartNew(() => { retValue[0] = getMD5(); }),
                    Task.Factory.StartNew(() => { retValue[1] = getRIPEMD160(); }),
                    Task.Factory.StartNew(() => { retValue[2] = getSHA1(); }),
                    Task.Factory.StartNew(() => { retValue[3] = getSHA256(); }),
                    Task.Factory.StartNew(() => { retValue[4] = getSHA384(); }),
                    Task.Factory.StartNew(() => { retValue[5] = getSHA512(); })
                };

                Task.WaitAll(taskArray[0], taskArray[1], taskArray[2], taskArray[3], taskArray[4], taskArray[5]);
                return retValue;
            }

            return getHash(filePath);
        }

        static byte[][] getHash(string filePath)
        { 
            byte[][] retValue = new byte[6][];

            try { 
                FileStream[] fsArray = new FileStream[6]
                {
                    new FileStream(filePath, FileMode.Open),
                    new FileStream(filePath, FileMode.Open),
                    new FileStream(filePath, FileMode.Open),
                    new FileStream(filePath, FileMode.Open),
                    new FileStream(filePath, FileMode.Open),
                    new FileStream(filePath, FileMode.Open)
                };

                retValue[0] = md5.ComputeHash(fsArray[0]);
                retValue[1] = ripemd160.ComputeHash(fsArray[1]);
                retValue[2] = sha1.ComputeHash(fsArray[2]);
                retValue[3] = sha256.ComputeHash(fsArray[3]);
                retValue[4] = sha384.ComputeHash(fsArray[4]);
                retValue[5] = sha512.ComputeHash(fsArray[5]);
                
                for (int y = 0; y < 6; ++y)
                {
                    fsArray[y].Close();
                    fsArray[y].Dispose();
                }

            }
            catch (Exception ex){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR - " + ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(0xFF);
            }
            return retValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte[] getMD5() { return md5.ComputeHash(FILE_DATA); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte[] getSHA1() { return sha1.ComputeHash(FILE_DATA);  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte[] getSHA256() { return sha256.ComputeHash(FILE_DATA); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte[] getSHA384() { return sha384.ComputeHash(FILE_DATA); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte[] getSHA512() { return sha512.ComputeHash(FILE_DATA); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte[] getRIPEMD160() { return ripemd160.ComputeHash(FILE_DATA); }
    }
}
