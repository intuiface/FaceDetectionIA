using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionIA
{
    public class Viewer : INotifyPropertyChanged
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

        #endregion

        #region Private Attributes

        private int m_iId;
        private string m_strGender, m_strAgeRange;
        private int m_iViewingTime, m_iDistance;
        private double m_iX, m_iY, m_iWidth, m_iHeight;

        //precision gender decision
        private double m_dMaleScore, m_dFemaleScore;
        private string m_strComputedGender;

        //main emotion
        private string m_strMainEmotion;
        private double m_dMainEmotionConfidence;

        //5 detected emotions & confidence level
        private double m_dAngerConfidence;
        private double m_dHappyConfidence;
        private double m_dNeutralConfidence;
        private double m_dSadConfidence;
        private double m_dSurpriseConfidence;



        #endregion

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

        public string ComputedGender
        {
            get { return m_strComputedGender; }
            set
            {
                if (m_strComputedGender != value)
                {
                    m_strComputedGender = value;
                    NotifyPropertyChanged("ComputedGender");
                }
            }
        }

        public double MaleScore
        {
            get { return m_dMaleScore; }
            set
            {
                if (m_dMaleScore != value)
                {
                    m_dMaleScore = value;
                    NotifyPropertyChanged("MaleScore");
                }
            }
        }

        public double FemaleScore
        {
            get { return m_dFemaleScore; }
            set
            {
                if (m_dFemaleScore != value)
                {
                    m_dFemaleScore = value;
                    NotifyPropertyChanged("FemaleScore");
                }
            }
        }

        public string AgeRange
        {
            get { return m_strAgeRange; }
            set
            {
                if (m_strAgeRange != value)
                {
                    m_strAgeRange = value;
                    NotifyPropertyChanged("AgeRange");
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

        public int Distance
        {
            get { return m_iDistance; }
            set
            {
                if (m_iDistance != value)
                {
                    m_iDistance = value;
                    NotifyPropertyChanged("Distance");
                }
            }
        }


        //main emotion


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

        //other emotions
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

        #endregion

        public override string ToString()
        {
            string res = Id + " -- " + Gender + " -- " + AgeRange + " -- " + ViewingTime + "\n";
            //res += X + " -- " + Y + " -- " + Width + " -- " + Height + "\n";
            return res;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Viewer other = obj as Viewer;
            if (object.ReferenceEquals(obj, null)|| other == null) // return false if obj is null OR if obj doesn't implement IUser
                return false;
            return (this.Id == other.Id);
        }
    }
}
