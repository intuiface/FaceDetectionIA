// ****************************************************************************
// <copyright file="FaceLostEventArgs.cs" company="Intuilab SAS">
// INTUILAB SAS
//_____________________
// [2002] - [2020] Intuilab SAS
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of Intuilab SAS.
// </copyright>
// ****************************************************************************

using System;
namespace FaceDetection.Events
{
    /// <summary>
    /// Class for event arguments.
    /// </summary>
    public class FaceLostEventArgs : EventArgs
    {
        #region Fields

        public int FaceId { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public double DwellTime { get; set; }

        #endregion Fields

        #region Constructor

        public FaceLostEventArgs(int faceId, string gender, string age, double dwellTime)
        {
            this.FaceId = faceId;
            this.Gender = gender;
            this.Age = age;
            this.DwellTime = dwellTime;
        }

        #endregion Constructor
    }
}
