using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using ResourceApplicationTool.Models;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;

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
        public  void SendMail(RATV3Entities db,
            Event ev, 
            List<Employee> attendantEmployees, 
            Employee creator,
            ControllerContext ControllerContext)
        {

            try
            {
                string embededHtml = "<html><head></head><body>" + "<p>Test</p>" + "<br/><br/>" + "<p>Test Embeded</p>" + "<br/>" + "<p>Test Ending</p>" + "<br/></body></html>";

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



                if(ev.EventType == "Performance Review" && ev.Attendants!= null && ev.Attendants.Count > 0)
                {
                    //we'll attach the pdf to the email

                    Attendant at = ev.Attendants.FirstOrDefault();
                    Employee reviewed = db.Employees.Where(x => x.EmployeeID == at.EmployeeID).FirstOrDefault();
                    if(reviewed != null)
                    {
                        Stream pdfDocument = GenerateUserCV(reviewed, ControllerContext);
                        if (pdfDocument != null)
                        {
                            LinkedResource resource = new LinkedResource(pdfDocument);
                            resource.ContentType.Name = reviewed.FirstName + " " + reviewed.LastName + " " + "Report.pdf";
                            htmlView.LinkedResources.Add(resource);
                        }
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



                AlternateView avCal = CreateICSView(email, ev.StartTime, endTime);
                //email.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                email.AlternateViews.Add(htmlView);
                email.AlternateViews.Add(avCal);

                //finally we send the mail
                client.Send(email);


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
        public  AlternateView CreateICSView(MailMessage email, DateTime startDate, DateTime endDate)
        {
            // Now Contruct the ICS file using string builder
            string str = CreateICSBody(email, startDate, endDate);
            System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
            contype.Parameters.Add("method", "REQUEST");
            contype.Parameters.Add("name", "Meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str, contype);

            return avCal;
        }
        /// <summary>
        /// Creates body of Outlook meeting notification
        /// </summary>
        public  string CreateICSBody(MailMessage email, DateTime startDate, DateTime endDate)
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
            str.AppendLine("LOCATION: Conference Call");
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("DESCRIPTION:{0}", email.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", email.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", email.Subject));
            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", email.From.Address));

            //attendees
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:MAILTO:\"{1}\"", email.To[0].DisplayName, email.To[0].Address));
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
    }
}