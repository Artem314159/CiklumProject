using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI;

namespace CiklumProject
{
    public partial class _Default : Page
    {
        const string SITE = "http://www.sportmaster.ua";
        const string MAIN_URL = "/catalog/vidi_turizma-pohodi-cholovchiy_odyag91";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string myConnection(string DBName = "Products")
        {
            StringBuilder SB = new StringBuilder();
            using (Connection newCon = new Connection("SELECT Id, Name, Price, PrcCurr FROM "+ DBName))
            {
                SB.Append("<caption>Товари</caption>");

                //SqlCommand mySqlCom = new SqlCommand(
                //    "SELECT COUNT(*) FROM information_schema.columns WHERE table_name = 'Products'", sqlCon);
                //SqlDataReader myReader = mySqlCom.ExecuteReader();
                //myReader.Read();
                int count = 4;//Convert.ToInt32(myReader.GetValue(0));
                              //myReader.Close();

                using (SqlDataReader reader = newCon.ExecuteReader())
                {
                    SB.Append("<tr>");
                    for (int i = 0; i < count; i++)
                    {
                        SB.Append("<th>" + reader.GetName(i) + "</th>");
                    }
                    SB.Append("</tr>");
                    while (reader.Read())
                    {
                        SB.Append("<tr>");
                        for (int i = 0; i < count; i++)
                        {
                            if(i==1)
                                SB.Append("<td><a runat=\"server\" href=\"View?productId=" +
                                    reader.GetValue(0) + "\">" + reader.GetValue(i) + "</td>");
                            else SB.Append("<td>" + reader.GetValue(i) + "</td>");
                        }
                        SB.Append("</tr>");
                    }
                    reader.Close();
                }
                newCon.Dispose();
            }
            return SB.ToString();
        }

        protected void GetProducts(object sender, EventArgs e)
        {
            using (Connection newCon = new Connection("DELETE FROM Products; DBCC CHECKIDENT " +
                "('Products', RESEED, 0);"))
            {
                newCon.ExecuteReader().Close();
                for (int i = 1; i <= 10; i++)
                {
                    string htmlCode = GetHtml(SITE + MAIN_URL + "/page-" + i);
                    Regex regexName = new Regex(@"full"">.*</div");
                    MatchCollection matchesNames = regexName.Matches(htmlCode);
                    Regex regexPrice = new Regex(@"bigger"">.*</div");
                    MatchCollection matchesPrice = regexPrice.Matches(htmlCode);
                    Regex regexHref = new Regex(@"<a href="".*box-name"">");
                    MatchCollection matchesHref = regexHref.Matches(htmlCode);

                    int count = matchesNames.Count;
                    for (int j = 0; j < count; j++)
                    {
                        string name = matchesNames[j].Value;
                        name = name.Substring(6, name.Length - 11);
                        if (name != null)
                        {
                            string[] priceMatch = matchesPrice[j].Value.Split(
                                new string[] { ", - ", ",- " }, StringSplitOptions.RemoveEmptyEntries);
                            double price = Convert.ToDouble(priceMatch[0].Substring(8, priceMatch[0].Length - 8));
                            string priceCurr = priceMatch[1].Substring(0, priceMatch[1].Length - 5);
                            string href = matchesHref[j].Value;
                            href = href.Substring(9, href.IndexOf("\" alt") - 9);

                            newCon.ChangeCommandText("INSERT INTO Products (Name, Price, PrcCurr, URL) " +
                                "VALUES " + "(N'" + name + "', " + price + ", N'" + priceCurr +
                                "', '" + (SITE + href) + "')");
                            newCon.ExecuteReader().Close();
                        }
                    }
                }
                //sqlCon.Close();
                newCon.Dispose();
            }
        }

        public static string GetHtml(string url)
        {
            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                htmlCode = client.DownloadString(url);
            }
            return htmlCode;
        }
    }

    public class Connection : IDisposable
    {
        SqlConnection sqlCon;
        SqlCommand mySqlCom;
        public Connection(string sqlQ)
        {
            string connS = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            sqlCon = new SqlConnection(connS);
            sqlCon.Open();
            mySqlCom = new SqlCommand(sqlQ, sqlCon);
        }

        public SqlDataReader ExecuteReader()
        {
            return mySqlCom.ExecuteReader();
        }
        public void ChangeCommandText(string newSqlQ)
        {
            mySqlCom.CommandText = newSqlQ;
        }

        public void Dispose()
        {
            sqlCon.Dispose();
        }
    }
}