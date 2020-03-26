// ****************************************************************************
// <copyright file="FaceDetection.cs" company="IntuiLab">
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

using FaceDetectionIA.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Timers;
using WebSocketSharp;

namespace FaceDetectionIA
{
    public class FaceDetection : INotifyPropertyChanged, IDisposable
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

        #region Private attributes

        // FaceDetection server IP
        private string m_strServerHost = "127.0.0.1";
        // FaceDetection server Port
        private int m_iServerPort = 2975;

        private WebSocket m_refWS;

        // Attributes to be displayed to IntuiFace
        private int m_iFaceCount = 0;
        private string m_strActivityLog = "";
        private ObservableCollection<Face> m_lstXPFaces;

        // Additional features
        private int m_dMinimumFaceSize = 2000; // width x height in pixels

        // FPS limiter
        private int m_iTimerThreshold = 500;

        // Main viewer
        private Face m_refMainFace;
        private bool m_bIsMainFaceDetected = false;

        private Timer m_refTimer;
        private bool m_bContinueListening = true;

        #endregion Private attributes

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
            get { return m_iServerPort; }
            set
            {
                if (m_iServerPort != value)
                {
                    m_iServerPort = value;
                    NotifyPropertyChanged("ServerPort");
                    _updateWS();
                }
            }
        }

        public int FaceCount
        {
            get { return m_iFaceCount; }
            set
            {
                if (m_iFaceCount != value)
                {
                    m_iFaceCount = value;
                    NotifyPropertyChanged("FaceCount");
                }
            }
        }

        public string ActivityLog
        {
            get { return m_strActivityLog; }
            set
            {
                if (m_strActivityLog != value)
                {
                    m_strActivityLog = value;
                    NotifyPropertyChanged("ActivityLog");
                }
            }
        }

        public ObservableCollection<Face> Faces
        {
            get { return m_lstXPFaces; }
            set
            {
                if (m_lstXPFaces != value)
                {
                    m_lstXPFaces = value;
                    NotifyPropertyChanged("Faces");
                }
            }
        }

        public int MinimumFaceSize
        {
            get { return m_dMinimumFaceSize; }
            set
            {
                if (m_dMinimumFaceSize != value)
                {
                    m_dMinimumFaceSize = value;
                    NotifyPropertyChanged("MinimumFaceSize");
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
                    {
                        m_refTimer.Interval = value;
                    }
                }
            }
        }

        public Face MainFace
        {
            get { return m_refMainFace; }
            set
            {
                if (m_refMainFace != value)
                {
                    m_refMainFace = value;
                    NotifyPropertyChanged("MainFace");
                }
            }
        }
        
        public bool IsMainFaceDetected
        {
            get { return m_bIsMainFaceDetected; }
            set
            {
                if (m_bIsMainFaceDetected != value)
                {
                    m_bIsMainFaceDetected = value;
                    NotifyPropertyChanged("IsMainFaceDetected");
                }
            }
        }

        #endregion Public Properties

        #region Events

        public delegate void FaceEventHandler(object sender, FaceEventArgs e);
        public event FaceEventHandler FaceDetected;
        public event FaceEventHandler FaceLost;

        protected void RaiseFaceDetected(int viewerId, string gender, string ageRange, int viewingTime)
        {
            FaceDetected(this, new FaceEventArgs(viewerId, gender, ageRange, viewingTime));
        }

        protected void RaiseFaceLost(int viewerId, string gender, string ageRange, int viewingTime)
        {
            FaceLost(this, new FaceEventArgs(viewerId, gender, ageRange, viewingTime));
        }

        #endregion Events

        #region Constructor

        public FaceDetection()
        {
            Faces = new ObservableCollection<Face>();
            MainFace = new Face() { Id = -1 };

            m_refTimer = new Timer(TimerThreshold);
            m_refTimer.Elapsed += Timer_Elapsed;

            _updateWS();
        }

        public void Dispose()
        {
            if (m_refWS != null && m_refWS.IsAlive)
            {
                m_refWS.OnOpen -= m_refWS_OnOpen;
                m_refWS.OnMessage -= m_refWS_OnMessage;
                m_refWS.OnError -= m_refWS_OnError;
                m_refWS.OnClose -= m_refWS_OnClose;
                m_refWS.Close();
            }
        }

        #endregion Constructor

        #region Private Operations


        #endregion Private Operations

        #region Callbacks

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_refTimer.Stop();
            m_bContinueListening = true;
        }

        void m_refWS_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Web socket open");
            ActivityLog += "Web socket open on " + ServerHost + ":" + ServerPort + "\n";
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

                // Case the list of viewers is empty                 
                if (o["viewers"] == null)
                    return;

                JArray ja = o["viewers"] as JArray;

                // Empty list addressed in _updateViewsList, to remove previous viewers. 
                _updateFacesList(ja);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                ActivityLog += ex.Message + "\n";
            }
        }

        private void _updateFacesList(JArray ja)
        {
            ObservableCollection<Face> newColl = new ObservableCollection<Face>();

            // Main viewer computation
            int mainFaceID = -1;
            float maxHeadSize = -1;

            // Copy new viewers in existing list            
            if (ja != null)
            {
                foreach (var v in ja)
                {
                    // Parse basic info
                    int id = int.Parse(v["id"].ToString());
                    string gender = v["gender"].ToString();
                    string age = v["age"].ToString();
                    var x = float.Parse(v["location"]["x"].ToString(), CultureInfo.InvariantCulture); // TODO: normalize? (or on C++ side?)
                    var y = float.Parse(v["location"]["y"].ToString(), CultureInfo.InvariantCulture);

                    var width = float.Parse(v["location"]["width"].ToString(), CultureInfo.InvariantCulture);
                    var height = float.Parse(v["location"]["height"].ToString(), CultureInfo.InvariantCulture);

                    // Intuiface coordinates system: move X&Y coords to represent the center of the head, not the top left corner
                    x += width / 2;
                    y += height / 2;

                    // TODO: estimate distance in cm based on head size and camera focal / calibration step ?? 
                    var distance = (int)(width * height * 100 * 100); // Current value: % of head area over total image area

                    var mainEmotion = v["mainEmotion"]["emotion"].ToString();
                    var mainEmotionConfidence = float.Parse(v["mainEmotion"]["confidence"].ToString(), CultureInfo.InvariantCulture);

                    // Parse additional emotions
                    var angerConfidence = float.Parse(v["emotions"]["anger"].ToString(), CultureInfo.InvariantCulture);
                    var happyConfidence = float.Parse(v["emotions"]["happy"].ToString(), CultureInfo.InvariantCulture);
                    var neutralConfidence = float.Parse(v["emotions"]["neutral"].ToString(), CultureInfo.InvariantCulture);
                    var sadConfidence = float.Parse(v["emotions"]["sad"].ToString(), CultureInfo.InvariantCulture);
                    var surpriseConfidence = float.Parse(v["emotions"]["surprise"].ToString(), CultureInfo.InvariantCulture);

                    // If face size filtering is active, check is head is bigger than threshold
                    if (distance < m_dMinimumFaceSize)
                    {
                        // Don't add face to list
                        continue;
                    }

                    // TODO: use real "distance" and take the min one. 
                    if (distance > maxHeadSize)
                    {
                        maxHeadSize = width * height;
                        mainFaceID = id;
                    }

                    newColl.Add(new Face()
                    {
                        Id = id,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                        FaceSize = distance,
                        Gender = gender,
                        Age = age,
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

            // Order by ID
            newColl = new ObservableCollection<Face>(newColl.OrderBy(i => i.Id));

            // Compare old (current) list and new list

            var removed = Faces.Except(newColl);
            var added = newColl.Except(Faces);

            if (removed.Count() > 0)
            {
                foreach (var face in removed)
                {
                    // Raise face lost event
                    Console.WriteLine("Face lost: " + face);
                    RaiseFaceLost(face.Id, face.Gender, face.Age, 0);
                }
            }
            if (added.Count() > 0)
            {
                foreach (var face in added)
                {
                    // Raise face added event
                    Console.WriteLine("Face added: " + face);
                    RaiseFaceDetected(face.Id, face.Gender, face.Age, 0);
                }
            }

            // Update elements in current list with new list

            int newCount = newColl.Count;
            int oldCount = Faces.Count;
            
            // Adjust number of viewers in Faces
            // New users to add
            if (newCount > oldCount)
            {
                for (int i = 0; i < newCount - oldCount; i++)
                {
                    Faces.Add(new Face());
                }
            }
            // Users to remove
            else if (newCount < oldCount)
            {
                for (int i = newCount; i < oldCount; i++)
                {
                    Faces.RemoveAt(newCount);
                }
            }

            // Copy new coll properties in existing viewers list.
            int index = 0;
            foreach (var item in newColl)
            {
                try
                {
                    Faces[index].Id = item.Id;
                    Faces[index].X = item.X;
                    Faces[index].Y = item.Y;
                    Faces[index].Width = item.Width;
                    Faces[index].Height = item.Height;
                    Faces[index].Gender = item.Gender;
                    Faces[index].Age = item.Age;
                    Faces[index].FaceSize = item.FaceSize;

                    Faces[index].MainEmotion = item.MainEmotion;
                    Faces[index].MainEmotionConfidence = item.MainEmotionConfidence;

                    // Additional emotions
                    Faces[index].AngerConfidence = item.AngerConfidence;
                    Faces[index].HappyConfidence = item.HappyConfidence;
                    Faces[index].NeutralConfidence = item.NeutralConfidence;
                    Faces[index].SadConfidence = item.SadConfidence;
                    Faces[index].SurpriseConfidence = item.SurpriseConfidence;
                }
                catch (Exception ex)
                {
                    ActivityLog += ex.Message + "\n";

                    throw;
                }
                
                index++;
            }

            // Update face count
            FaceCount = Faces.Count();

            // Copy main viewer info
            if (FaceCount > 0)
            {
                var item = Faces.Single(i => i.Id == mainFaceID);
                MainFace.Id = item.Id;
                MainFace.X = item.X;
                MainFace.Y = item.Y;
                MainFace.Width = item.Width;
                MainFace.Height = item.Height;
                MainFace.FaceSize = item.FaceSize;
                MainFace.Gender = item.Gender;

                MainFace.Age = item.Age;
                MainFace.MainEmotion = item.MainEmotion;
                MainFace.MainEmotionConfidence = item.MainEmotionConfidence;

                // Additional emotions
                MainFace.AngerConfidence = item.AngerConfidence;
                MainFace.HappyConfidence = item.HappyConfidence;
                MainFace.NeutralConfidence = item.NeutralConfidence;
                MainFace.SadConfidence = item.SadConfidence;
                MainFace.SurpriseConfidence = item.SurpriseConfidence;

                // Mark the viewer detected
                IsMainFaceDetected = true;
            }
            else
            {
                IsMainFaceDetected = false;
            }
        }

        private void _removeFace(int id)
        {
            try
            {
                int index = -1;

                for (int i = 0; i < Faces.Count; i++)
                {
                    if (Faces[i].Id == id)
                    {
                        index = i;
                        break;
                    }
                }

                // Index found
                if (index != -1)
                {
                    Face v = Faces[index];

                    // Raise event first
                    RaiseFaceLost(v.Id, v.Gender, v.Age, 0);

                    // Remove index from map and Face from list
                    Faces.RemoveAt(index);
                    FaceCount = Faces.Count;

                    Console.WriteLine("Remove viewer " + id + " at index " + index);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("REMOVE VIEWER ERROR: " + ex.Message);
                ActivityLog += "REMOVE VIEWER ERROR:" + ex.Message + "\n";
            }
        }

        void m_refWS_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {            
            ActivityLog += "REMOVE VIEWER ERROR:" + e.Message + "\n";            
        }

        void m_refWS_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            ActivityLog += "WebSocket Closed:" + e.Reason + "\n";
            System.Threading.Thread.Sleep(5000);
            _updateWS();
            m_refWS.Connect();
        }

        private void _updateWS()
        {
            if (m_refWS != null && m_refWS.IsAlive)
            {
                m_refWS.OnOpen -= m_refWS_OnOpen;
                m_refWS.OnMessage -= m_refWS_OnMessage;
                m_refWS.OnError -= m_refWS_OnError;
                m_refWS.OnClose -= m_refWS_OnClose;
                m_refWS.Close();
            }
            m_refWS = new WebSocket("ws://" + m_strServerHost + ":" + m_iServerPort);

            m_refWS.OnOpen += new EventHandler(m_refWS_OnOpen);
            m_refWS.OnMessage += new EventHandler<MessageEventArgs>(m_refWS_OnMessage);
            m_refWS.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(m_refWS_OnError);
            m_refWS.OnClose += new EventHandler<WebSocketSharp.CloseEventArgs>(m_refWS_OnClose);
        }

        #endregion Callbacks

        #region Public Operations

        public void ConnectToServer()
        {
            ActivityLog += "Trying to open Web socket on " + ServerHost + ":" + ServerPort + "\n";
            m_refWS.Connect();            
        }

        public void DisconnectFromServer()
        {
            ActivityLog += "Trying to close Web socket on " + ServerHost + ":" + ServerPort + "\n";
            m_refWS.Close();
        }

        #endregion Public Operations
    }
}
