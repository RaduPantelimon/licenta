using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class CalendarEvent
    {
        public CalendarEvent() {
        }
        public CalendarEvent(Event e)
        {
            start = e.StartTime.ToString("yyyy-MM-ddThh:mm:ss");
            end = e.StartTime.ToString("yyyy-MM-ddThh:mm:ss");
            title = e.Title;
            url = "/Events/Details/" + e.EventID;
            id = e.EventID;
        }
        public CalendarEvent(Event e, int currentUserID)
        {
            start = e.StartTime.ToString("yyyy-MM-ddThh:mm:ss");
            end = e.StartTime.ToString("yyyy-MM-ddThh:mm:ss");
            title = e.Title;
            url = "/Events/Details/" + e.EventID;
            id = e.EventID;

            if(e.CreatorID == currentUserID)
            {
                color = "#ff7f7f";
            }
            else
            {
                color = "#337ab7";
            }
        }
        public string start;
        public string end;
        public string title;
        public string url;
        public string color;
        public int id;
    }
}