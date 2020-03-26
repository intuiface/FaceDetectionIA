// ****************************************************************************
// <copyright file="Viewer.cs" company="IntuiLab">
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
using System.ComponentModel;

namespace FaceDetectionIA
{
    /// <summary>
    /// Face informations
    /// </summary>
    public class Face : INotifyPropertyChanged
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

        private int m_iId;
        private string m_strGender, m_strAge;
        private int m_iViewingTime, m_iFaceSize;
        private double m_iX, m_iY, m_iWidth, m_iHeight;

        // Main emotion
        private string m_strMainEmotion;
        private double m_dMainEmotionConfidence;

        // 5 detected emotions & confidence level
        private double m_dAngerConfidence;
        private double m_dHappyConfidence;
        private double m_dNeutralConfidence;
        private double m_dSadConfidence;
        private double m_dSurpriseConfidence;

        #endregion Private Attributes

        #region Public Properties

        public int Id
        {
            get { return m_iId; }
            set
            {
                if (m_iId != value)
                {
                    m_iId = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        public string Gender
        {
            get { return m_strGender; }
            set
            {
                if (m_strGender != value)
                {
                    m_strGender = value;
                    NotifyPropertyChanged("Gender");
                }
            }
        }

        public string Age
        {
            get { return m_strAge; }
            set
            {
                if (m_strAge != value)
                {
                    m_strAge = value;
                    NotifyPropertyChanged("Age");
                }
            }
        }


        public int ViewingTime
        {
            get { return m_iViewingTime; }
            set
            {
                if (m_iViewingTime != value)
                {
                    m_iViewingTime = value;
                    NotifyPropertyChanged("ViewingTime");
                }
            }
        }

        public double X
        {
            get { return m_iX; }
            set
            {
                if (m_iX != value)
                {
                    m_iX = value;
                    NotifyPropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get { return m_iY; }
            set
            {
                if (m_iY != value)
                {
                    m_iY = value;
                    NotifyPropertyChanged("Y");
                }
            }
        }

        public double Width
        {
            get { return m_iWidth; }
            set
            {
                if (m_iWidth != value)
                {
                    m_iWidth = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }

        public double Height
        {
            get { return m_iHeight; }
            set
            {
                if (m_iHeight != value)
                {
                    m_iHeight = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public int FaceSize
        {
            get { return m_iFaceSize; }
            set
            {
                if (m_iFaceSize != value)
                {
                    m_iFaceSize = value;
                    NotifyPropertyChanged("FaceSize");
                }
            }
        }


        // Main emotion

        public string MainEmotion
        {
            get { return m_strMainEmotion; }
            set
            {
                if (m_strMainEmotion != value)
                {
                    m_strMainEmotion = value;
                    NotifyPropertyChanged("MainEmotion");
                }
            }
        }

        public double MainEmotionConfidence
        {
            get { return m_dMainEmotionConfidence; }
            set
            {
                if (m_dMainEmotionConfidence != value)
                {
                    m_dMainEmotionConfidence = value;
                    NotifyPropertyChanged("MainEmotionConfidence");
                }
            }
        }

        // Other emotions
        public double AngerConfidence
        {
            get { return m_dAngerConfidence; }
            set
            {
                if (m_dAngerConfidence != value)
                {
                    m_dAngerConfidence = value;
                    NotifyPropertyChanged("AngerConfidence");
                }
            }
        }

        public double SurpriseConfidence
        {
            get { return m_dSurpriseConfidence; }
            set
            {
                if (m_dSurpriseConfidence != value)
                {
                    m_dSurpriseConfidence = value;
                    NotifyPropertyChanged("SurpriseConfidence");
                }
            }
        }

        public double HappyConfidence
        {
            get { return m_dHappyConfidence; }
            set
            {
                if (m_dHappyConfidence != value)
                {
                    m_dHappyConfidence = value;
                    NotifyPropertyChanged("HappyConfidence");
                }
            }
        }

        public double NeutralConfidence
        {
            get { return m_dNeutralConfidence; }
            set
            {
                if (m_dNeutralConfidence != value)
                {
                    m_dNeutralConfidence = value;
                    NotifyPropertyChanged("NeutralConfidence");
                }
            }
        }

        public double SadConfidence
        {
            get { return m_dSadConfidence; }
            set
            {
                if (m_dSadConfidence != value)
                {
                    m_dSadConfidence = value;
                    NotifyPropertyChanged("SadConfidence");
                }
            }
        }

        #endregion Public Properties

        #region Operations

        public override string ToString()
        {
            string res = Id + " -- " + Gender + " -- " + Age + " -- " + ViewingTime + "\n";
            return res;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Face other = obj as Face;

            // return false if obj is null OR if obj doesn't implement IUser
            if (object.ReferenceEquals(obj, null)|| other == null)
            {
                return false;
            }
            return (this.Id == other.Id);
        }

        #endregion Operations
    }
}
