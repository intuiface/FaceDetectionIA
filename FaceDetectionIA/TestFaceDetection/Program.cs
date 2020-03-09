using FaceDetectionIA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFaceDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceDetection fd = new FaceDetection();
            fd.StartListening();

            string q = "";
            while (q != "q")
            {
                q = Console.ReadLine();
                switch (q)
                {
                    case "wall":
                        fd.ServerHost = "10.0.0.2";
                        fd.StartListening();
                    break;
                    default:
                        break;
                }

            }
        }
    }
}
