using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;
using ResourceApplicationTool.Models;
using Newtonsoft.Json;
using System.Text;

namespace ResourceApplicationTool.Controllers
{
    public class RestApiController : ApiController
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Sprints for project
        public HttpResponseMessage GetSprints(int id)
        {
            var result = db.Sprints.Where(x => x.ProjectID == id);
            var jsonResponseText = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");

            return response;
        }

        // GET: Employees for project
        public HttpResponseMessage GetEmployees(int id)
        {
            string jsonResponseText = "";
            Project project = db.Projects.Where(x => x.ProjectID == id).FirstOrDefault();
            if (project != null)
            {
                var result = db.Employees.Where(x => x.DepartmentID == project.DepartmentID);
                jsonResponseText = JsonConvert.SerializeObject(result);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");

            return response;
        }

        // GET: Template Tasks
        public HttpResponseMessage GetTemplateTasks()
        {
            string jsonResponseText = "";
            List<Task> tasks = db.Tasks.Where(x => !(x.SprintID.HasValue) ).ToList();
            if (tasks != null)
            {
                jsonResponseText = JsonConvert.SerializeObject(tasks);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");

            return response;
        }


        //GET: Sprint Tasks
        public HttpResponseMessage GetSprintTasks(int id)
        {
            string jsonResponseText = "";
            Sprint sprint = db.Sprints.Where(x => x.SprintID == id).FirstOrDefault();
            if(sprint != null)
            {
                List<Task> tasks = db.Tasks.Where(x => x.SprintID == sprint.SprintID).ToList();
                if (tasks != null)
                {
                    jsonResponseText = JsonConvert.SerializeObject(tasks);
                }
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
            return response;
        }

        //save the tasks added by the user
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage SaveTask([FromBody] TaskTemplate data)
        {
            string jsonResponseText = "";
            try
            {
                Task templateTask = db.Tasks.Where(x => x.TaskID == data.templateTaskID).FirstOrDefault();
                if(templateTask != null)
                {
                    DateTime startDate = DateTime.Parse(data.startDate);
                    Task task = new Task();

                    task.TaskDescription = templateTask.TaskDescription;
                    task.SprintID = data.sprintID;
                    task.EmployeeID = data.employeeID;
                    task.StartDate = startDate;
                    task.EndDate = startDate;
                    task.Difficulty = templateTask.Difficulty;
                    db.Tasks.Add(task);
                    db.SaveChanges();

                    jsonResponseText = JsonConvert.SerializeObject(task);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
                return response;
            }
            catch(Exception ex)
            {
                jsonResponseText = "{\"status\":0,\"error\":\"Error trying to create the new task\",\"message\":\"" + ex.Message + "\"}";
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
                return response;
            }
            
        }

        //deletes a task
        [System.Web.Http.AcceptVerbs("DELETE")]
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteTask(int id)
        {
            string jsonResponseText = "";
            try
            {
                Task task = db.Tasks.Where(x => x.TaskID == id).FirstOrDefault();
                if (task != null)
                {
                    db.Tasks.Remove(task);
                    db.SaveChanges();

                    jsonResponseText = "{\"status\":1,\"message\":\"Task was successfully deleted.\"}";
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                jsonResponseText = "{\"status\":0,\"error\":\"Error trying to create the new task\",\"message\":\"" + ex.Message + "\"}";
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(jsonResponseText, Encoding.UTF8, "application/json");
                return response;
            }

        }

    }
}
