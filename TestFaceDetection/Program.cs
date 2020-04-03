// ****************************************************************************
// <copyright file="Program.cs" company="Intuilab SAS">
// INTUILAB SAS
//_____________________
// [2002] - [2020] Intuilab SAS
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of Intuilab SAS.
// </copyright>
// ****************************************************************************

using FaceDetection;
using System;
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
