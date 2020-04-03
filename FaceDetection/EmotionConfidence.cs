// ****************************************************************************
// <copyright file="EmotionConfidence.cs" company="Intuilab SAS">
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
    /// Emotion informations
    /// </summary>
    public class EmotionConfidence : INotifyPropertyChanged
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

        // 5 detected emotions & confidence level
        private double m_dAngry;
        private double m_dHappy;
        private double m_dNeutral;
        private double m_dSad;
        private double m_dSurprised;

        #endregion Private Attributes

        #region Public Properties

        public double Angry
        {
            get { return m_dAngry; }
            set
            {
                if (m_dAngry != value)
                {
                    m_dAngry = value;
                    NotifyPropertyChanged("Angry");
                }
            }
        }

        public double Surprised
        {
            get { return m_dSurprised; }
            set
            {
                if (m_dSurprised != value)
                {
                    m_dSurprised = value;
                    NotifyPropertyChanged("Surprised");
                }
            }
        }

        public double Happy
        {
            get { return m_dHappy; }
            set
            {
                if (m_dHappy != value)
                {
                    m_dHappy = value;
                    NotifyPropertyChanged("Happy");
                }
            }
        }

        public double Neutral
        {
            get { return m_dNeutral; }
            set
            {
                if (m_dNeutral != value)
                {
                    m_dNeutral = value;
                    NotifyPropertyChanged("Neutral");
                }
            }
        }

        public double Sad
        {
            get { return m_dSad; }
            set
            {
                if (m_dSad != value)
                {
                    m_dSad = value;
                    NotifyPropertyChanged("Sad");
                }
            }
        }

        #endregion Public Properties
    }
}
