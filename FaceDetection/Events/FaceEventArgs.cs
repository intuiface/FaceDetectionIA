// ****************************************************************************
// <copyright file="FaceEventArgs.cs" company="IntuiLab">
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
