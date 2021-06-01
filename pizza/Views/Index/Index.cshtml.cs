using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pizza.Classes;
using Microsoft.AspNetCore.Http;

namespace pizza.Views
{
    public class IndexModel : PageModel
    {
        public List<Pizza> listPizza;
        public List<string> param;
        public List<string> paramOfExeptions;

        public int count=0;
        public int selectedMin;
        public int selectedMax;
        public List<string> selectedParams;
        public List<string> selectedParamOfExeptions;

        public string incr()
        {
            count++;
            return "";
        }
        public IndexModel()
        {
            selectedParams = new List<string>();
            listPizza = new List<Pizza>();
            listPizza = Pizza.selectFromDb();
            param = new List<string>();
            paramOfExeptions = new List<string>();
            selectedParamOfExeptions = new List<string>();


            #region params
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
                        param.Add(reader[1].ToString().Trim());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

            #region params
            connectionString = "Data Source=DESKTOP-QGEEUPD;Initial Catalog=Pizza;Integrated Security=True";

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
                        paramOfExeptions.Add(reader[1].ToString().Trim());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion
        }
        public void selectByParam(List<string> list, int from, int to, List<string> list2)
        {
            string where = "";
            Console.WriteLine(from + ":" + to);
            if (to != 0)
            {
                where = "price>" + from.ToString() + " and price<" + to.ToString();
            }
            Console.WriteLine(where);
            listPizza = new List<Pizza>();
            listPizza = Pizza.selectFromDb(where);
            if (list.Count != 0|| list2.Count != 0)
            {
                for (int i = 0; i < listPizza.Count; i++)
                {
                    bool b = true;
                    foreach (var item in list)
                    {
                        if (!listPizza[i].Ingradients.items.Contains(item))
                        {
                            b = false;

                        }
                    }
                    foreach (var itemTmp in list2)
                    {
                        List<string> iList = getExept(itemTmp);
                        foreach (var item in iList)
                        {
                        if (item[0].ToString() != "!")
                        {
                            if (!listPizza[i].Ingradients.items.Contains(new string(item)))
                            {
                                b = false;

                            }
                        }
                        else
                        {
                            if (listPizza[i].Ingradients.items.Contains(new string(item.Skip(1).ToArray())))
                            {
                                b = false;

                            }
                        }


                        }
                    }
                    if (!b)
                    {
                        listPizza.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        public List<string> getExept(string item)
        {
            List<string> list = new List<string>();

            string connectionString = "Data Source=DESKTOP-QGEEUPD;Initial Catalog=Pizza;Integrated Security=True";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command;
                // Create the Command and Parameter objects.
                    command = new SqlCommand("select * from pizza_exeptions where pizza_exeptions.[name]='"+item+"'", connection);
                
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list=reader[2].ToString().Trim().Split(", ").ToList();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return list;
        }
    }
}
