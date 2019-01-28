using System;
using System.IO;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

namespace Repro.AWS
{
    class Program
    {
        private const string AwsAccessKey = "/* TUTAJ API KEY */";
        private const string AwsSecretKey = "/* TUTAJ SECRET KEY */";
        private static readonly AmazonS3Config S3Config = new AmazonS3Config
        {
            ServiceURL = "http://e24files.com",
            MaxErrorRetry = 2
        };

        static void Main(string[] args)
        {
            var message = "Testowa wiadomość. !!!";
            var bytes = Encoding.UTF8.GetBytes(message);

            // Working
            using (var stream = new MemoryStream(bytes))
            {
                var path = "plik.txt";
                var bucketName = "testowy.1";

                Write(path, stream, bucketName);
            }

            // Exception
            using (var stream = new MemoryStream(bytes))
            {
                var path = "plik(1).txt";
                var bucketName = "testowy.1";

                Write(path, stream, bucketName);
            }

            Console.ReadKey();
        }

        static void Write(string path, Stream stream, string bucketName)
        {
            try
            {
                using (var client = GetClient())
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = path,
                        AutoCloseStream = false,
                        InputStream = stream
                    };

                    client.PutObject(request);

                    //var request = new DeleteObjectRequest() { BucketName = bucketName, Key = path };
                    //client.DeleteObject(request);

                    Console.WriteLine($"Pomyślnie zapisano plik {path} w storage bucket.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nie udało się zapisać pliku {path} w storage bucket.");

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static IAmazonS3 GetClient()
        {
           return new AmazonS3Client(AwsAccessKey, AwsSecretKey, S3Config);
        }
    }
}
