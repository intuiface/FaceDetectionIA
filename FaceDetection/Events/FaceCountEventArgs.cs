﻿// ****************************************************************************
// <copyright file="FaceCountEventArgs.cs" company="Intuilab SAS">
// INTUILAB CONFIDENTIAL
//_____________________
// [2002] - [2020] Intuilab SAS
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of Intuilab SAS. The intellectual and technical
// concepts contained herein are proprietary to Intuilab SAS
// and may be covered by U.S. and other country Patents, patents
// in process, and are protected by trade secret or copyright law.
// Dissemination of this information or reproduction of this
// material is strictly forbidden unless prior written permission
// is obtained from Intuilab SAS.
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
