//using demojqgrid.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demojqgrid.Controllers
{
    public class JqgridController : Controller
    {
        // GET: Jqgrid
        public ActionResult Index()
        {
            return View();
        }

        StudentsEntities db = new StudentsEntities();
        public JsonResult GetValues(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString) //Gets the todo Lists.  
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var Results = db.StudentTables.Select(
                a => new
                {
                    a.Id,
                    a.Name,
                    a.StudentClass,

                });

            if (_search)
            {
                switch (searchField)
                {
                    case "Name":
                        Results = Results.Where(t => t.Name.Contains(searchString));
                        break;
                    case "StudentClass":
                        Results = Results.Where(t => t.StudentClass.Contains(searchString));
                        break;
                }
            }

            int totalRecords = Results.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sord.ToUpper() == "DESC")
            {
                Results = Results.OrderByDescending(s => s.Id);
                Results = Results.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                Results = Results.OrderBy(s => s.Id);
                Results = Results.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = Results
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        // TODO:insert a new row to the grid logic here  
        [HttpPost]
        public string Create([Bind(Exclude = "Id")] StudentTable obj)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    db.StudentTables.Add(obj);
                    db.SaveChanges();
                    msg = "Saved Successfully";
                }
                else
                {
                    msg = "Validation data not successfull";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        public string Edit(StudentTable obj)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = "Saved Successfully";
                }
                else
                {
                    msg = "Validation data not successfull";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        public string Delete(int Id)
        {
            StudentTable list = db.StudentTables.Find(Id);
            db.StudentTables.Remove(list);
            db.SaveChanges();
            return "Deleted successfully";
        }
    }
}