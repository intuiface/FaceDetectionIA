// ****************************************************************************
// <copyright file="FaceEventArgs.cs" company="Intuilab SAS">
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
    public class FaceEventArgs : EventArgs
    {
        #region Fields

        public int FaceId { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }

        #endregion Fields

        #region Constructor

        public FaceEventArgs(int faceId, string gender, string age)
        {
            this.FaceId = faceId;
            this.Gender = gender;
            this.Age = age;
        }

        #endregion Constructor
    }
}
