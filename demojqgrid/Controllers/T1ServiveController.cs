using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using demojqgrid.Helpers;

namespace demojqgrid.Controllers
{
    public class T1ServiveController : Controller
    {
        private readonly JsonSerializerOptions jsonSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public JsonResult GetValues(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString) //Gets the todo Lists.  
        {
            var Results = new List<T1ServiceModel>();

            var baseModel = new BaseGridModel();
            baseModel.search = searchString;
            baseModel.pagenumber = page;
            baseModel.pagesize = rows;

            string data = JsonSerializer.Serialize(baseModel);
            string url = string.Format("{0}T1Service/GetServiceControllerData_Count", "http://localhost:11977/api/");
            var recordCountResponse = HttpHelper.SendRequest(string.Empty, HttpMethod.Post, url, data).GetAwaiter().GetResult();
            if (recordCountResponse.StatusCode == HttpStatusCode.OK)
            {
                url = string.Format("{0}T1Service/ServiceDataGetAll_New", "http://localhost:11977/api/");
                var response = HttpHelper.SendRequest(string.Empty, HttpMethod.Post, url, data).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Results = JsonSerializer.Deserialize<List<T1ServiceModel>>(response.RawResponse, this.jsonSerializationOptions);
                   
                }
            }
             
            if (_search)
            {
                switch (searchField)
                {
                    case "Name":
                        Results = Results.Where(t => t.ProgramName.Contains(searchString)).ToList();
                        break;
                    case "StudentClass":
                        Results = Results.Where(t => t.Value.Contains(searchString)).ToList();
                        break;
                }
            }

            int totalRecords = Convert.ToInt16(recordCountResponse.RawResponse);
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            //if (sord.ToUpper() == "DESC")
            //{
            //    Results = Results.OrderByDescending(s => s.Id);
            //    Results = Results.Skip(pageIndex * pageSize).Take(pageSize);
            //}
            //else
            //{
            //    Results = Results.OrderBy(s => s.Id);
            //    Results = Results.Skip(pageIndex * pageSize).Take(pageSize);
            //}
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = Results
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}