// ****************************************************************************
// <copyright file="Program.cs" company="IntuiLab">
// INTUILAB CONFIDENTIAL
//_____________________
// [2002] - [2020] IntuiLab
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of IntuiLab. The intellectual and technical
// concepts contained herein are proprietary to IntuiLab
// and may be covered by U.S. and other country Patents, patents
// in process, and are protected by trade secret or copyright law.
// Dissemination of this information or reproduction of this
// material is strictly forbidden unless prior written permission
// is obtained from IntuiLab.
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
