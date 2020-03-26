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

using FaceDetection.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Timers;
using WebSocketSharp;

namespace FaceDetection
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

        // Main face
        private Face m_refMainFace;
        private bool m_bIsMainFaceDetected = false;

        private Timer m_refTimer;
        private bool m_bContinueListening = true;

        private bool m_bIsConnectedToFaceDetectionServer = false;

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

        public bool IsConnectedToFaceDetectionServer
        {
            get { return m_bIsConnectedToFaceDetectionServer; }
            set
            {
                if (m_bIsConnectedToFaceDetectionServer != value)
                {
                    m_bIsConnectedToFaceDetectionServer = value;
                    NotifyPropertyChanged("IsConnectedToFaceDetectionServer");
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
        public delegate void FaceCountEventHandler(object sender, FaceCountEventArgs e);

        public event FaceEventHandler FaceDetected;
        public event FaceEventHandler FaceLost;
        public event FaceCountEventHandler FaceCountChanged;

        protected void RaiseFaceDetected(int faceId, string gender, string ageRange, int viewingTime)
        {
            FaceDetected(this, new FaceEventArgs(faceId, gender, ageRange, viewingTime));
        }

        protected void RaiseFaceLost(int faceId, string gender, string ageRange, int viewingTime)
        {
            FaceLost(this, new FaceEventArgs(faceId, gender, ageRange, viewingTime));
        }

        protected void RaiseFaceCountChanged(int count)
        {
            FaceCountChanged(this, new FaceCountEventArgs(count));
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
            IsConnectedToFaceDetectionServer = true;

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

                // Case the list of faces is empty                 
                if (o["faces"] == null)
                    return;

                JArray ja = o["faces"] as JArray;

                // Empty list addressed in _updateFacesList, to remove previous faces. 
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

            // Main face computation
            int mainFaceID = -1;
            float maxHeadSize = -1;

            // Copy new faces in existing list            
            if (ja != null)
            {
                foreach (var v in ja)
                {
                    // Parse basic info
                    int id = int.Parse(v["id"].ToString());
                    string gender = v["gender"].ToString();

                    string age = v["age"].ToString();
                    var ageInteger = Int16.Parse(age);
                    string ageRange = ageInteger <= 16 ? "child" :
                                ageInteger <= 30 ? "young adult" :
                                ageInteger <= 45 ? "middle-aged adult" : "old-aged adult";

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
                    EmotionConfidence emotionConfidence = new EmotionConfidence();
                    emotionConfidence.Angry = float.Parse(v["emotions"]["anger"].ToString(), CultureInfo.InvariantCulture);
                    emotionConfidence.Happy = float.Parse(v["emotions"]["happy"].ToString(), CultureInfo.InvariantCulture);
                    emotionConfidence.Neutral = float.Parse(v["emotions"]["neutral"].ToString(), CultureInfo.InvariantCulture);
                    emotionConfidence.Sad = float.Parse(v["emotions"]["sad"].ToString(), CultureInfo.InvariantCulture);
                    emotionConfidence.Surprised = float.Parse(v["emotions"]["surprise"].ToString(), CultureInfo.InvariantCulture);

                    // Parse head pose estimation
                    HeadPoseEstimation headPoseEstimation = new HeadPoseEstimation();
                    headPoseEstimation.Pitch = float.Parse(v["headpose"]["pitch"].ToString(), CultureInfo.InvariantCulture);
                    headPoseEstimation.Yaw = float.Parse(v["headpose"]["yaw"].ToString(), CultureInfo.InvariantCulture);
                    headPoseEstimation.Roll = float.Parse(v["headpose"]["roll"].ToString(), CultureInfo.InvariantCulture);

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
                        AgeRange = ageRange,
                        MainEmotion = mainEmotion,
                        MainEmotionConfidence = mainEmotionConfidence,
                        EmotionConfidence = emotionConfidence,
                        HeadPoseEstimation = headPoseEstimation
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

            if (newCount != oldCount)
            {
                // Raise face count changed event
                Console.WriteLine("Face count changed: " + newCount);
                RaiseFaceCountChanged(newCount);
            }

            // Adjust number of faces in Faces
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

            // Copy new coll properties in existing faces list.
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
                    Faces[index].AgeRange = item.AgeRange;
                    Faces[index].FaceSize = item.FaceSize;

                    Faces[index].MainEmotion = item.MainEmotion;
                    Faces[index].MainEmotionConfidence = item.MainEmotionConfidence;

                    // Additional emotions
                    Faces[index].EmotionConfidence = item.EmotionConfidence;

                    // Head pose estimation
                    Faces[index].HeadPoseEstimation = item.HeadPoseEstimation;
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

            // Copy main face info
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
                MainFace.AgeRange = item.AgeRange;
                MainFace.MainEmotion = item.MainEmotion;
                MainFace.MainEmotionConfidence = item.MainEmotionConfidence;

                // Additional emotions
                MainFace.EmotionConfidence = item.EmotionConfidence;

                // Head pose estimation
                MainFace.HeadPoseEstimation = item.HeadPoseEstimation;

                // Mark the face detected
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

                    Console.WriteLine("Remove faces " + id + " at index " + index);
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
            IsConnectedToFaceDetectionServer = false;

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
