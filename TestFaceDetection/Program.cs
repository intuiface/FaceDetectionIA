using FaceDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TestFaceDetection
{
    class Program
    {
        static FaceDetection.FaceDetection fd;

        static void Main(string[] args)
        {
            fd = new FaceDetection.FaceDetection();
            fd.ConnectToServer();

            string q = "";
            while (q != "q")
            {
                q = Console.ReadLine();
            }

            fd.Dispose();
            fd = null;
        }
    }
}
