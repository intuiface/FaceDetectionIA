using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionIA.Events
{
    public class ViewerEventArgs : EventArgs
    {
        public int ViewerId { get; set; }
        public string Gender { get; set; }
        public string AgeRange { get; set; }
        public int ViewingTime { get; set; }


        public ViewerEventArgs(int viewerId, string gender, string ageRange, int viewingTime)
        {
            this.ViewerId = viewerId;
            this.Gender = gender;
            this.AgeRange = ageRange;
            this.ViewingTime = viewingTime;
        }
    }
}
