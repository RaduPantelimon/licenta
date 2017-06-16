using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.SecondaryModels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
namespace ResourceApplicationTool.Utils
{
    public class Mailer
    {

       public SmtpClient client;

       public Mailer()
        {
            client = new SmtpClient(Const.MAILER.SERVER, Const.MAILER.PORT)
            {
                Credentials = new NetworkCredential(Const.MAILER.EMAIL_ACCOUNT, Const.MAILER.EMAIL_PASSWORD),
                EnableSsl = true
            };
        }


        /// <summary>
        ///  //we'll get the email addresses and notify the attendants the attendants
        /// </summary>
        public  void SendMeetingRequest(RATV3Entities db,
            Event ev, 
            List<Employee> attendantEmployees, 
            Employee creator,
            ControllerContext ControllerContext,
            int update = 0)
        {

            try
            {
                //getting the reviewed employee
                Attendant at = ev.Attendants.FirstOrDefault();
                Employee reviewed = db.Employees.Where(x => x.EmployeeID == at.EmployeeID).FirstOrDefault();
                reviewed.SkillLevelsList = reviewed.SkillLevels.ToList();

                //getting message content
                EventTypeInfo currentEventType = Const.EventTypesinfo.Where(x => x.EventType == ev.EventType).FirstOrDefault();
                string embededHtml = "<html><head></head><body>" + "<p>Test</p>" + "<br/><br/>" + "<p>Test Embeded</p>" + "<br/>" + "<p>Test Ending</p>" + "<br/></body></html>";

                if(currentEventType != null && currentEventType.EventType == "Performance Review")
                {
                    string generatednotificationHTML = ViewRenderer.RenderView("~/Views/Notifications/PerformanceReview.cshtml", reviewed,
                                                 ControllerContext);

                    if(!String.IsNullOrEmpty(generatednotificationHTML))
                    {
                        embededHtml = generatednotificationHTML;
                    }
                }
                else if (currentEventType != null && currentEventType.EventType == "Department Monthly Meeting")
                {
                    string generatednotificationHTML = ViewRenderer.RenderView("~/Views/Notifications/DepartmentMonthlyMeeting.cshtml", reviewed,
                                                 ControllerContext);

                    if (!String.IsNullOrEmpty(generatednotificationHTML))
                    {
                        embededHtml = generatednotificationHTML;
                    }
                }


                //preping the email message
                MailMessage email = new MailMessage();
                email.From = new MailAddress(creator.Email, creator.FirstName + ' ' + creator.LastName);
                //adding recipients
                foreach (Employee attendant in attendantEmployees)
                {
                    email.To.Add(new MailAddress(attendant.Email, attendant.FirstName + ' ' + attendant.LastName));
                }

                email.IsBodyHtml = true;
                email.Subject = ev.EventType;


                //preparing email content
                //"text/html" - this view will have all the content
                System.Net.Mime.ContentType htmlMimeContent = new System.Net.Mime.ContentType("text/html");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(embededHtml, htmlMimeContent);
                htmlView.ContentType.CharSet = Encoding.UTF8.WebName;
                if (ev.EventType == "Performance Review" && ev.Attendants!= null && ev.Attendants.Count > 0 && reviewed != null)
                {
                    //we'll attach the pdf to the email

                        Stream pdfDocument = GenerateUserCV(reviewed, ControllerContext);
                        if (pdfDocument != null)
                        {
                            LinkedResource resource = new LinkedResource(pdfDocument);
                            resource.ContentType.Name = reviewed.FirstName + " " + reviewed.LastName + " " + "Report.pdf";
                            htmlView.LinkedResources.Add(resource);
                        }     
                }
                else if (ev.EventType == "Department Monthly Meeting" && reviewed.DepartmentID.HasValue)
                {
                    //we'll attach the excel report to the email

                    byte[] array = ExcelReportGenerator.GenerateExcelReportForDepartment(reviewed.DepartmentID.Value, 
                            ev.StartTime.Month, ev.StartTime.Year, db);
                    Stream excelDocument = new MemoryStream(array);
                    if (excelDocument != null)
                    {
                        LinkedResource resource = new LinkedResource(excelDocument);
                        resource.ContentType.Name = "Department Report.xlsx";
                        htmlView.LinkedResources.Add(resource);
                    }

                }




                //preparing calendar meeting view
                DateTime endTime;
                if (ev.EndTime.HasValue)
                {
                    endTime = ev.EndTime.Value;
                }
                else
                {
                    endTime = ev.StartTime;
                }

                //this is the guid of the meeting request
                Guid requestGUID;

                if (ev.IcsGuid.HasValue)
                {
                    requestGUID = ev.IcsGuid.Value;
                }
                else
                {
                    requestGUID = Guid.NewGuid();
                }
                AlternateView avCal = CreateICSView(email, ev.StartTime, endTime, ev, requestGUID,update);
                //email.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                email.AlternateViews.Add(htmlView);
                email.AlternateViews.Add(avCal);

                //finally we send the mail
                client.Send(email);

                if (!ev.IcsGuid.HasValue)
                {
                    ev.IcsGuid = requestGUID;
                    db.Entry(ev).State = EntityState.Modified;
                }
                if (update>0)
                {
                    //we want to be able to store the update number too

                    //ev.UpdateNo = update;
                    db.Entry(ev).State = EntityState.Modified;
                }
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                //handle Exception
            }

        }
        public Stream GenerateUserCV(Employee employee,
            ControllerContext ControllerContext)
        {
            //string generatedPDFHtml = this.RenderView("PdfCVGenerator", employee, ViewData);
            string generatedPDFHtml = ViewRenderer.RenderView("~/Views/Employees/PdfCVGenerator.cshtml", employee,
                                                 ControllerContext);
            //generating the header
            string headertext = ViewRenderer.RenderView("~/Views/Employees/PdfCVHeader.cshtml", employee,
                                                 ControllerContext);

            byte[] pdfBuffer = PdfGenerator.ConvertHtmlToPDF(generatedPDFHtml, headertext);

            Stream stream = new MemoryStream(pdfBuffer);

            return stream;
        }


        public  System.Threading.Tasks.Task SendAsync(SmtpClient client, MailMessage message)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Guid sendGuid = Guid.NewGuid();

            SendCompletedEventHandler handler = null;
            handler = (o, ea) =>
            {
                if (ea.UserState is Guid && ((Guid)ea.UserState) == sendGuid)
                {
                    client.SendCompleted -= handler;
                    if (ea.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (ea.Error != null)
                    {
                        tcs.SetException(ea.Error);
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                }
            };

            client.SendCompleted += handler;
            client.SendAsync(message, sendGuid);
            return tcs.Task;
        }


        /// <summary>
        /// Creates Outlook meeting notification
        /// </summary>
        public  AlternateView CreateICSView(MailMessage email, DateTime startDate, DateTime endDate,Event ev,  Guid requestGUID,int update = 0)
        {
            // Now Contruct the ICS file using string builder
            string str = CreateICSBody(email, startDate, endDate,ev, requestGUID, update);
            System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
            contype.Parameters.Add("method", "REQUEST");
            contype.Parameters.Add("name", "Meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str, contype);

            return avCal;
        }
        /// <summary>
        /// Creates body of Outlook meeting notification
        /// </summary>
        public  string CreateICSBody(MailMessage email, DateTime startDate, DateTime endDate, Event ev,  Guid requestGUID, int update = 0)
        {

            string location = String.IsNullOrEmpty(ev.Location)?"Conference Call":ev.Location;            


            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Schedule a Meeting");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");
            str.AppendLine("BEGIN:VEVENT");
            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startDate));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));

            if (update > 0)
            {
                str.AppendLine(string.Format("SEQUENCE:{0}",update));
            }

            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endDate));
            str.AppendLine("LOCATION:" + location);
            str.AppendLine(string.Format("UID:{0}", requestGUID));
            
            str.AppendLine(string.Format("DESCRIPTION:{0}", email.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", email.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", email.Subject));
            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", email.From.Address));

            //attendees
            foreach(MailAddress m in email.To)
            {
                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:MAILTO:\"{1}\"", m.DisplayName, m.Address));

            }
            //str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:MAILTO:\"{1}\"", email.To[1].DisplayName, email.To[1].Address));
            //str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", email.To[0].DisplayName, email.To[0].Address));

            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:DISPLAY");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            return str.ToString();
        }

        /// <summary>
        ///  //we'll get the email addresses and notify the attendants about the event being canceled
        /// </summary>
        public void CancelMeetingRequest(RATV3Entities db,
            Event ev,
            List<Employee> attendantEmployees,
            Employee creator,
            ControllerContext ControllerContext)
        {

            //getting the reviewed employee
            Attendant at = ev.Attendants.FirstOrDefault();
            Employee reviewed = db.Employees.Where(x => x.EmployeeID == at.EmployeeID).FirstOrDefault();
            reviewed.SkillLevelsList = reviewed.SkillLevels.ToList();


            string embededHtml = ViewRenderer.RenderView("~/Views/Notifications/CanceledEvent.cshtml", reviewed,
                                            ControllerContext);

            if (String.IsNullOrEmpty(embededHtml))
            {
                embededHtml = " This Event has been canceled";
            }
            //preping the email message
            var email = new MailMessage();
            email.From = new MailAddress(creator.Email, creator.FirstName + ' ' + creator.LastName);
            //adding recipients
            foreach (Employee attendant in attendantEmployees)
            {
                email.To.Add(new MailAddress(attendant.Email, attendant.FirstName + ' ' + attendant.LastName));
            }

            email.IsBodyHtml = true;
            email.Subject = ev.EventType;


            //preparing email content
            //"text/html" - this view will have all the content
            System.Net.Mime.ContentType htmlMimeContent = new System.Net.Mime.ContentType("text/html");
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(embededHtml, htmlMimeContent);
            htmlView.ContentType.CharSet = Encoding.UTF8.WebName;

            //preparing calendar meeting view
            DateTime endTime;
            if (ev.EndTime.HasValue)
            {
                endTime = ev.EndTime.Value;
            }
            else
            {
                endTime = ev.StartTime;
            }

            //this is the guid of the meeting request
            Guid requestGUID;

            if (ev.IcsGuid.HasValue)
            {
                //if we have 
                requestGUID = ev.IcsGuid.Value;
                AlternateView avCal = CreateCancelationICSView(email, ev.StartTime, endTime, ev, requestGUID);
                //email.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                email.AlternateViews.Add(htmlView);
                email.AlternateViews.Add(avCal);

                //finally we send the mail
                client.Send(email);
            }

        }


        /// <summary>
        /// Creates Outlook meeting notification
        /// </summary>
        public AlternateView CreateCancelationICSView(MailMessage email, DateTime startDate, DateTime endDate, Event ev, Guid requestGUID)
        {
            // Now Contruct the ICS file using string builder
            string str = CreateCancelledICSBody(email, startDate, endDate, ev, requestGUID);
            System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
            contype.Parameters.Add("method", "REQUEST");
            contype.Parameters.Add("name", "Meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str, contype);

            return avCal;
        }


        /// <summary>
        /// Creates body of Outlook meeting cancelation notification
        /// </summary>
        public string CreateCancelledICSBody(MailMessage email, DateTime startDate, DateTime endDate, Event ev, Guid requestGUID)
        {

            

            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Schedule a Meeting");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");
            str.AppendLine("BEGIN:VEVENT");
            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startDate));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));

            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endDate));
            str.AppendLine(string.Format("UID:{0}", requestGUID));

            str.AppendLine(string.Format("DESCRIPTION:{0}", email.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", email.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", email.Subject));
            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", email.From.Address));

            //attendees
            foreach (MailAddress m in email.To)
            {
                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:MAILTO:\"{1}\"", m.DisplayName, m.Address));

            }

            str.AppendLine("STATUS:CANCELLED");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            return str.ToString();
        }


    }
}