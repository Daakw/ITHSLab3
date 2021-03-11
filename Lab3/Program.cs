using System;
using System.IO;
using System.Linq;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine($"Write your own path or choose to copy one of the following images");
            Console.WriteLine($"../../../Dice.png\n../../../Mario.png\n../../../Test1.bmp\n../../../Test2.bmp");
            Console.Write($"\nInsert path to image: ");
            var pathInput = Console.ReadLine();
            
            try
            {
                FileType(pathInput);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Could not find any file");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"\nEmpty path!");
            }


            Console.ReadKey();

        }
        public static void FileType(string path) 
        {
            byte[] buffer;
            byte[] pngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };
            byte[] bmpSignature = { 66, 77 };

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
            }

            var pngBytes = buffer.Take(8);
            var bmpBytes = buffer.Take(2);

            if (pngBytes.SequenceEqual(pngSignature))
            {
                var width = BitConverter.ToInt32(buffer.Skip(16).Take(4).Reverse().ToArray());
                var height = BitConverter.ToInt32(buffer.Skip(20).Take(4).Reverse().ToArray());
                Console.WriteLine($"PNG width = {width}, height = {height}");

                var headerLength = 8; //File header
                var lengthLength = 4; //Length
                var chunkLength = 4; //Chunk type
                var checksumLength = 4; //CRC
                var index = headerLength;
                var p = 1;
                while (buffer.Length > headerLength) 
                {
                    var dataLength = BitConverter.ToInt32(buffer.Skip(index).Take(lengthLength).Reverse().ToArray());
                    var chunkType = System.Text.Encoding.ASCII.GetString(buffer.Skip(index + lengthLength).Take(chunkLength).ToArray());
                    index += lengthLength + chunkLength + dataLength + checksumLength;

                    Console.WriteLine($"{p}. {chunkType}, Bytes = {dataLength}");
                    p++;
                }
            }
            else if (bmpBytes.SequenceEqual(bmpSignature))
            {
                Console.WriteLine("This is a bmp-file");
                var width = BitConverter.ToInt32(buffer.Skip(18).Take(4).ToArray());
                var height = BitConverter.ToInt32(buffer.Skip(22).Take(4).ToArray());
                Console.WriteLine($"Width = {width}, Height = {height}");

            }
            else
            {
                Console.WriteLine("This is not a valid .bmp or .png-file!");
            }


        }



    }

}
