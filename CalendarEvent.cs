using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyScheduler
{
    [Serializable]
    public class CalendarEvent
    {
        private DateTime startTime;
        private DateTime endTime;
        private string txt;
        private Color color;

        public CalendarEvent(DateTime startTime, DateTime endTime, string txt, Color color)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.txt = txt;
            this.color = color;
        }

        public CalendarEvent()
        {

        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public string Text
        {
            get { return txt; }
            set { txt = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public string[] getElements()
        {
            string[] str_arr = { startTime.ToString("HH:mm")+"-"+endTime.ToString("HH:mm"), txt };
            return str_arr;
        }
    }
}
