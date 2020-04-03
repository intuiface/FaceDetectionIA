// ****************************************************************************
// <copyright file="HeadPoseEstimation.cs" company="Intuilab SAS">
// INTUILAB SAS
//_____________________
// [2002] - [2020] Intuilab SAS
// All Rights Reserved.
// NOTICE: All information contained herein is, and remains
// the property of Intuilab SAS.
// </copyright>
// ****************************************************************************

using System;
using System.ComponentModel;

namespace FaceDetection
{
    /// <summary>
    /// Head pose informations
    /// </summary>
    public class HeadPoseEstimation : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion INotifyPropertyChanged

        #region Private Attributes

        private double m_dPitch, m_dYaw, m_dRoll;

        #endregion Private Attributes

        #region Public Properties

        public double Pitch
        {
            get { return m_dPitch; }
            set
            {
                if (m_dPitch != value)
                {
                    m_dPitch = value;
                    NotifyPropertyChanged("Pitch");
                }
            }
        }

        public double Yaw
        {
            get { return m_dYaw; }
            set
            {
                if (m_dYaw != value)
                {
                    m_dYaw = value;
                    NotifyPropertyChanged("Yaw");
                }
            }
        }

        public double Roll
        {
            get { return m_dRoll; }
            set
            {
                if (m_dRoll != value)
                {
                    m_dRoll = value;
                    NotifyPropertyChanged("Roll");
                }
            }
        }

        #endregion Public Properties
    }
}
