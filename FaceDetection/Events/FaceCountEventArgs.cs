// ****************************************************************************
// <copyright file="FaceCountEventArgs.cs" company="Intuilab SAS">
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
    public class FaceCountEventArgs : EventArgs
    {
        #region Fields

        public int Count { get; set; }

        #endregion Fields

        #region Constructor

        public FaceCountEventArgs(int count)
        {
            this.Count = count;
        }

        #endregion Constructor
    }
}
