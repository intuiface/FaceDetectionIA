using FaceDetectionIA.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebSocketSharp;

namespace FaceDetectionIA
{
    public class FaceDetection : INotifyPropertyChanged
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

        #region Private attributes

        // FaceDetection server IP
        private string m_strServerHost = "127.0.0.1";
        // FaceDetection server Port
        private int m_iPort = 2975;
        
        private WebSocket m_refWS;

        //Attributes to be displayed to IntuiFace
        private int m_iCurrentCount = 0;        
        private string m_strLogText = "";
        private bool m_bActivateLogs = false;
        //private HashSet<int> m_mapCurrentViewerIds;
        private ObservableCollection<Viewer> m_lstXPViewers;

        //additional features
        private bool m_bFilterHeadSize = false;
        private int m_dHeadSizeThreashold = 2000; // width x height in pixels

        private double m_dGenderScoreThreshold = 0.8;

        //fps limiter
        private int m_iTimerThreshold = 500;

        //main viewer
        private Viewer m_refMainViewer;
        private bool m_bIsMainViewerDetected = false;

        #endregion

        #region Public Properties

        public string ServerHost
        {
            get { return m_strServerHost; }
            set
            {
                if (m_strServerHost != value)
                {
                    m_strServerHost = value;
                    NotifyPropertyChanged("ServerHost");
                    _updateWS();
                }
            }
        }

        public int ServerPort
        {
            get { return m_iPort; }
            set
            {
                if (m_iPort != value)
                {
                    m_iPort = value;
                    NotifyPropertyChanged("ServerPort");
                    _updateWS();
                }
            }
        }

        public bool ActivateLogs
        {
            get { return m_bActivateLogs; }
            set
            {
                if (m_bActivateLogs != value)
                {
                    m_bActivateLogs = value;
                    NotifyPropertyChanged("ActivateLogs");
                }
            }
        }

        public int CurrentCount
        {
            get { return m_iCurrentCount; }
            set
            {
                if (m_iCurrentCount != value)
                {
                    m_iCurrentCount = value;
                    NotifyPropertyChanged("CurrentCount");
                }
            }
        }

      
        public string LogText
        {
            get { return m_strLogText; }
            set
            {
                if (m_strLogText != value)
                {
                    m_strLogText = value;
                    NotifyPropertyChanged("LogText");
                }
            }
        }

        public ObservableCollection<Viewer> CurrentViewers
        {
            get { return m_lstXPViewers; }
            set
            {
                if (m_lstXPViewers != value)
                {
                    m_lstXPViewers = value;
                    NotifyPropertyChanged("CurrentViewers");
                }
            }
        }

        public bool FilterHeadSize
        {
            get { return m_bFilterHeadSize; }
            set
            {
                if (m_bFilterHeadSize != value)
                {
                    m_bFilterHeadSize = value;
                    NotifyPropertyChanged("FilterHeadSize");
                }
            }
        }

        public int HeadSizeThreashold
        {
            get { return m_dHeadSizeThreashold; }
            set
            {
                if (m_dHeadSizeThreashold != value)
                {
                    m_dHeadSizeThreashold = value;
                    NotifyPropertyChanged("HeadSizeThreashold");
                }
            }
        }

        public int TimerThreshold
        {
            get { return m_iTimerThreshold; }
            set
            {
                if (m_iTimerThreshold != value)
                {
                    m_iTimerThreshold = value;
                    NotifyPropertyChanged("TimerThreshold");
                    if (m_refTimer != null)
                        m_refTimer.Interval = value;
                }
            }
        }

        public double GenderScoreThreshold
        {
            get { return m_dGenderScoreThreshold; }
            set
            {
                if (m_dGenderScoreThreshold != value)
                {
                    m_dGenderScoreThreshold = value;
                    NotifyPropertyChanged("GenderScoreThreshold");
                }
            }
        }

        public Viewer MainViewer
        {
            get { return m_refMainViewer; }
            set
            {
                if (m_refMainViewer != value)
                {
                    m_refMainViewer = value;
                    NotifyPropertyChanged("MainViewer");                    
                }
            }
        }


        public bool IsMainViewerDetected
        {
            get { return m_bIsMainViewerDetected; }
            set
            {
                if (m_bIsMainViewerDetected != value)
                {
                    m_bIsMainViewerDetected = value;
                    NotifyPropertyChanged("IsMainViewerDetected");
                }
            }
        }

        #endregion

        #region Events

        public delegate void ViewerEventHandler(object sender, ViewerEventArgs e);
        public event ViewerEventHandler ViewerDetected;
        public event ViewerEventHandler ViewerLost;
        public event ViewerEventHandler MaleDetected, FemaleDetected;

        protected void RaiseViewerDetected(int viewerId, string gender, string ageRange, int viewingTime)
        {
            if (ViewerDetected != null)
            {
                ViewerDetected(this, new ViewerEventArgs(viewerId, gender, ageRange, viewingTime));
            }
        }

        protected void RaiseViewerLost(int viewerId, string gender, string ageRange, int viewingTime)
        {
            if (ViewerLost != null)
            {
                ViewerLost(this, new ViewerEventArgs(viewerId, gender, ageRange, viewingTime));
            }
        }


        protected void RaiseMaleDetected(int viewerId, string gender, string ageRange, int viewingTime)
        {
            if (MaleDetected != null)
            {
                MaleDetected(this, new ViewerEventArgs(viewerId, gender, ageRange, viewingTime));
            }
        }

        protected void RaiseFemaleDetected(int viewerId, string gender, string ageRange, int viewingTime)
        {
            if (FemaleDetected != null)
            {
                FemaleDetected(this, new ViewerEventArgs(viewerId, gender, ageRange, viewingTime));
            }
        }

        #endregion

        #region Constructor

        private Timer m_refTimer;
        bool m_bContinueListening = true;

        public FaceDetection()
        {
            //m_mapCurrentViewerIds = new HashSet<int>();
            CurrentViewers = new ObservableCollection<Viewer>();
            MainViewer = new Viewer()
            {
                Id = -1
            };

            //m_refWS = new WebSocket("ws://" + m_strServerHost + ":" + m_iPort, m_strServiceName);
            m_refWS = new WebSocket("ws://" + m_strServerHost + ":" + m_iPort);

            m_refWS.OnOpen += new EventHandler(m_refWS_OnOpen);
            m_refWS.OnMessage += new EventHandler<MessageEventArgs>(m_refWS_OnMessage);
            m_refWS.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(m_refWS_OnError);

            m_refTimer = new Timer(TimerThreshold);
            m_refTimer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_refTimer.Stop();
            m_bContinueListening = true;
        }

        #endregion

        #region callbacks

        void m_refWS_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Web socket open");
            LogText += "Web socket open on " + ServerHost + ":" + ServerPort + "\n";
        }

        void m_refWS_OnMessage(object sender, MessageEventArgs e)
        {

            if (!m_bContinueListening)
            {
                return;
            }

            m_bContinueListening = false;
            m_refTimer.Start();

            try
            {
                JObject o = JObject.Parse(e.Data);

                //Case the list of viewers is empty                 
                if (o["viewers"] == null)
                    return;

                JArray ja = o["viewers"] as JArray;

                //empty list addressed in _updateViewsList, to remove previous viewers. 
                _updateViewersList(ja);
        }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                //Debugger.Break();

                if (ActivateLogs)
                {
                    LogText += ex.Message + "\n";
                }
}
        }

        private void _updateViewersList(JArray ja)
        {
          
            ObservableCollection<Viewer> newColl = new ObservableCollection<Viewer>();

            //main viewer computation
            int mainViewerID = -1;
            float maxHeadSize = -1;


            //copy new viewers in existing list            
            if (ja != null)
            {
                foreach (var v in ja)
                {
                    //parse basic info
                    int id = int.Parse(v["id"].ToString());
                    string gender = v["gender"].ToString();
                    string age = v["age"].ToString();
                    var x = float.Parse(v["location"]["x"].ToString(), CultureInfo.InvariantCulture); // TODO: normalize? (or on C++ side?)
                    var y = float.Parse(v["location"]["y"].ToString(), CultureInfo.InvariantCulture); // 

                    var width = float.Parse(v["location"]["width"].ToString(), CultureInfo.InvariantCulture); // 
                    var height = float.Parse(v["location"]["height"].ToString(), CultureInfo.InvariantCulture); // 

                    //Intuiface coordinates system: move X&Y coords to represent the center of the head, not the top left corner
                    x += width / 2;
                    y += height / 2;

                    //TEMP computation
                    //TODO: estimate distance in cm based on head size and camera focal / calibration step ?? 
                    var distance = (int)(width * height * 100 * 100); //Current value: % of head area over total image area

                    //prevision gender
                    var maleScore = float.Parse(v["maleScore"].ToString(), CultureInfo.InvariantCulture);
                    var femaleScore = float.Parse(v["femaleScore"].ToString(), CultureInfo.InvariantCulture);
                    string computedGender = _computeGender(maleScore, femaleScore);

                    var mainEmotion = v["mainEmotion"]["emotion"].ToString();
                    var mainEmotionConfidence = float.Parse(v["mainEmotion"]["confidence"].ToString(), CultureInfo.InvariantCulture);

                    //parse additional emotions
                    var angerConfidence = float.Parse(v["emotions"]["anger"].ToString(), CultureInfo.InvariantCulture);
                    var happyConfidence = float.Parse(v["emotions"]["happy"].ToString(), CultureInfo.InvariantCulture);
                    var neutralConfidence = float.Parse(v["emotions"]["neutral"].ToString(), CultureInfo.InvariantCulture);
                    var sadConfidence = float.Parse(v["emotions"]["sad"].ToString(), CultureInfo.InvariantCulture);
                    var surpriseConfidence = float.Parse(v["emotions"]["surprise"].ToString(), CultureInfo.InvariantCulture);

                    //if face size filtering is active, check is head is bigger than threshold
                    if (m_bFilterHeadSize && distance < m_dHeadSizeThreashold)
                    {
                        //don't add face to list
                        continue;
                    }

                    //test main viewer
                    //SME: currently uses "distance field as head size", to compare to max head size
                    //TODO: use real "distance" and take the min one. 
                    if (distance > maxHeadSize)
                    {
                        maxHeadSize = width * height;
                        mainViewerID = id;
                    }

                    newColl.Add(new Viewer()
                    {
                        Id = id,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                        Distance = distance,
                        Gender = gender,
                        MaleScore = maleScore,
                        FemaleScore = femaleScore,
                        ComputedGender = computedGender,
                        AgeRange = age,
                        MainEmotion = mainEmotion,
                        MainEmotionConfidence = mainEmotionConfidence,
                        AngerConfidence = angerConfidence,
                        HappyConfidence = happyConfidence,
                        NeutralConfidence = neutralConfidence,
                        SadConfidence = sadConfidence,
                        SurpriseConfidence = surpriseConfidence

                    });                             
                }

            }
            //order by ID
            newColl = new ObservableCollection<Viewer>(newColl.OrderBy(i => i.Id));

            //compare old (current) list and new list

            var removed = CurrentViewers.Except(newColl);
            var added = newColl.Except(CurrentViewers);

            if (removed.Count() > 0)
            {
                foreach (var v in removed)
                {
                    //Raise viewer lost event
                    Console.WriteLine("Viewer lost: " + v);
                    //var v = CurrentViewers.Single(i => i.Id == item);
                    RaiseViewerLost(v.Id, v.Gender, v.AgeRange, 0);
                }
            }
            if (added.Count()> 0)
            {
                foreach (var v in added)
                {
                    //Raise viewer added event
                    Console.WriteLine("Viewer added: " + v);
                    RaiseViewerDetected(v.Id, v.Gender, v.AgeRange, 0);
                }
            }

            //update elements in current list with new list

            int newCount = newColl.Count;
            int oldCount = CurrentViewers.Count;
            
            //adjust number of viewers in CurrentViewers
            //new users to add
            if (newCount > oldCount)
            {
                for (int i = 0; i < newCount - oldCount; i++)
                {
                    CurrentViewers.Add(new Viewer());
                }
            }
            //users to remove
            else if (newCount < oldCount)
            {
                for (int i = newCount; i < oldCount; i++)
                {
                    CurrentViewers.RemoveAt(newCount);
                }
            }

            //TEST
            if (CurrentViewers.Count != newCount)
            {
                Debugger.Break();
            }


            //copy new coll properties in existing viewers list. 
            int index = 0;
            foreach (var item in newColl)
            {
                try
                {
                    CurrentViewers[index].Id = item.Id;
                    CurrentViewers[index].X = item.X;
                    CurrentViewers[index].Y = item.Y;
                    CurrentViewers[index].Width = item.Width;
                    CurrentViewers[index].Height = item.Height;
                    CurrentViewers[index].Distance = item.Distance;
                    CurrentViewers[index].Gender = item.Gender;
                    CurrentViewers[index].MaleScore = item.MaleScore;
                    CurrentViewers[index].FemaleScore = item.FemaleScore;
                    CurrentViewers[index].ComputedGender = item.ComputedGender;

                    CurrentViewers[index].AgeRange = item.AgeRange;
                    CurrentViewers[index].MainEmotion = item.MainEmotion;
                    CurrentViewers[index].MainEmotionConfidence = item.MainEmotionConfidence;

                    //additional emotions
                    CurrentViewers[index].AngerConfidence = item.AngerConfidence;
                    CurrentViewers[index].HappyConfidence = item.HappyConfidence;
                    CurrentViewers[index].NeutralConfidence = item.NeutralConfidence;
                    CurrentViewers[index].SadConfidence = item.SadConfidence;
                    CurrentViewers[index].SurpriseConfidence = item.SurpriseConfidence;
                }
                catch (Exception ex)
                {
                    if (ActivateLogs)
                    {
                        LogText += ex.Message + "\n";
                    }
                    throw;
                }
                
                index++;
            }

            //update current count
            CurrentCount = CurrentViewers.Count();

            //copy main viewer info
            if (CurrentCount > 0)
            {
                var item = CurrentViewers.Single(i => i.Id == mainViewerID);
                MainViewer.Id = item.Id;
                MainViewer.X = item.X;
                MainViewer.Y = item.Y;
                MainViewer.Width = item.Width;
                MainViewer.Height = item.Height;
                MainViewer.Distance = item.Distance;
                MainViewer.Gender = item.Gender;

                //precise gender score
                MainViewer.MaleScore = item.MaleScore;
                MainViewer.FemaleScore = item.FemaleScore;
                MainViewer.ComputedGender = item.ComputedGender;

                MainViewer.AgeRange = item.AgeRange;
                MainViewer.MainEmotion = item.MainEmotion;
                MainViewer.MainEmotionConfidence = item.MainEmotionConfidence;

                //additional emotions
                MainViewer.AngerConfidence = item.AngerConfidence;
                MainViewer.HappyConfidence = item.HappyConfidence;
                MainViewer.NeutralConfidence = item.NeutralConfidence;
                MainViewer.SadConfidence = item.SadConfidence;
                MainViewer.SurpriseConfidence = item.SurpriseConfidence;

                //mark the viewer detected
                IsMainViewerDetected = true;
            }
            else
                IsMainViewerDetected = false;
        }

        private void _removeViewer(int id)
        {


            try
            {
                int index = -1;

                for (int i = 0; i < CurrentViewers.Count; i++)
                {
                    if (CurrentViewers[i].Id == id)
                    {
                        index = i;
                        break;
                    }
                }

                //index found
                if (index != -1)
                {
                    Viewer v = CurrentViewers[index];
                    //raise event first
                    RaiseViewerLost(v.Id, v.Gender, v.AgeRange, 0);

                    //remove index from map and Viewer from list. 
                    //m_mapCurrentViewerIds.Remove(id);
                    CurrentViewers.RemoveAt(index);
                    CurrentCount = CurrentViewers.Count;

                    Console.WriteLine("Remove viewer " + id + " at index " + index);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("REMOVE VIEWER ERROR: " + ex.Message);
                LogText += "REMOVE VIEWER ERROR:" + ex.Message + "\n";
            }         
        }

        void m_refWS_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {            
            LogText += "REMOVE VIEWER ERROR:" + e.Message + "\n";            
        }


        private string _computeGender(double maleScore, double femaleScore)
        {
            string res = "unknown";
            double threshold = 0.80;
            if (maleScore > threshold)
                res = "male";
            if (femaleScore > threshold)
                res = "female";
            
            return res;
        }

        private void _updateWS()
        {
            if (m_refWS != null && m_refWS.IsAlive)
            {
                m_refWS.Close();
                m_refWS.OnOpen -= m_refWS_OnOpen;
                m_refWS.OnMessage -= m_refWS_OnMessage;
                m_refWS.OnError -= m_refWS_OnError;
            }
            m_refWS = new WebSocket("ws://" + m_strServerHost + ":" + m_iPort);

            m_refWS.OnOpen += new EventHandler(m_refWS_OnOpen);
            m_refWS.OnMessage += new EventHandler<MessageEventArgs>(m_refWS_OnMessage);
            m_refWS.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(m_refWS_OnError);
        }

        #endregion

        #region public methods

        public void StartListening()
        {
            LogText += "Trying to open Web socket on " + ServerHost + ":" + ServerPort + "\n";
            m_refWS.Connect();
            
        }

        public void StopListening()
        {
            LogText += "Trying to close Web socket on " + ServerHost + ":" + ServerPort + "\n";
            m_refWS.Close();
        }

        public void Refresh()
        {
            m_lstXPViewers.Clear();
            //m_mapCurrentViewerIds.Clear();

            CurrentCount = 0;
        }


        #endregion

    }
}
