using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pizza.Views;
using pizza.Views.Index;
using System.Data.SqlClient;
using pizza.Classes;
using Microsoft.AspNetCore.Http;

namespace pizza.Controllers
{
    public class IndexController:Controller
    {
        IHttpContextAccessor accessor;
        private List<string> funk(IHttpContextAccessor accessor)
        {
            List<string> l = new List<string>();



            string connectionString = "Data Source=DESKTOP-QGEEUPD;Initial Catalog=Pizza;Integrated Security=True";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("select * from [standart_choise]", connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        l.Add(reader[1].ToString().Trim());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            List<string> end = new List<string>();
            for(int i=0;i<l.Count ;i++ )
            {
                if (accessor.HttpContext.Request.Query[l[i]].ToString()=="on")
                {
                    end.Add(l[i]);
                }
            }
            return end;
        }

        private List<string> funk2(IHttpContextAccessor accessor)
        {
            List<string> l = new List<string>();



            string connectionString = "Data Source=DESKTOP-QGEEUPD;Initial Catalog=Pizza;Integrated Security=True";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("select * from [pizza_exeptions]", connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        l.Add(reader[1].ToString().Trim());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            List<string> end = new List<string>();
            for (int i = 0; i < l.Count; i++)
            {
                if (accessor.HttpContext.Request.Query[l[i]].ToString() == "on")
                {
                    end.Add(l[i]);
                }
            }
            return end;
        }
        public IndexController(IHttpContextAccessor _accessor)
        {
            accessor = _accessor;
        }
        public IActionResult Index(int? from, int? to)
        {
            IndexModel model = new IndexModel();


            int fromE = 0;
            int toE = 999999;
            List<string> list;
            list = funk(accessor);

            List<string> list2;
            list2 = funk2(accessor);
            if (from != null)
            {
                fromE = from.Value;
                model.selectedMin = fromE;
            }
            if (to != null)
            {
                toE = to.Value;
                model.selectedMax = toE;
            }
            model.selectedParams = list;
             model.selectByParam(list, fromE, toE, list2);

            return View(model);
        }
        public IActionResult OwnChoise()
        {
            OwnChoiseModel model = new OwnChoiseModel();
            return View(model);
        }
        [HttpPost]
        public IActionResult SaveOrder(int id, string richnes, string size, string components)
        {
            Pizza l=Pizza.selectFromDb(id);
            
            string connectionString = "Data Source=DESKTOP-QGEEUPD;Initial Catalog=Pizza;Integrated Security=True";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command;
                if (components != null) 
                {
                    command = new SqlCommand("insert into[orders](size, richnes, components) values('" + size + "', '" + richnes + "', '" + l.Ingradients.stringedItems + ", " + components + "');", connection);
                }
                else
                {
                    command = new SqlCommand("insert into[orders](size, richnes, components) values('" + size + "', '" + richnes + "', '" + l.Ingradients.stringedItems +"');", connection);
                }
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("id: "+id+"\nRichnes: "+richnes+"\nSize: "+size);
            return Redirect("~/");
        }
        [HttpPost]
        public IActionResult SaveOwnOrder(string size, string components, string richnes)
        {
            string connectionString = "Data Source=DESKTOP-QGEEUPD;Initial Catalog=Pizza;Integrated Security=True";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                if(components.Split(", ").Length<=1)
                {
                    var a= components.Split(',');
                    components = "";
                    components += a[0];
                    for(int i=1;i<a.Length ;i++ )
                    {
                        components += ", "+a[i];
                    }
                }
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("insert into[orders](size, richnes, components) values('"+size+"', '"+ richnes + "', '"+components+"');", connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("own");

            Console.WriteLine("Components: " + components + "\nSize: " + size+"\nRichnes: "+ richnes);
            return Redirect("~/");
        }
    }
}
