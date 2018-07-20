using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace CiklumProject
{
    public partial class WebForm1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //function for wrong Id
        public void IdNotFound(bool change = true)
        {
            if (change)
            {
                notFound.Style.Add("display", "normal");
                mainContent.Style.Add("display", "none");
            }
        }

        public void GetProduct()
        {
            Directory.SetCurrentDirectory(HttpRuntime.AppDomainAppPath);
            string currUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            try
            {
                ushort productId = Convert.ToUInt16(HttpUtility.ParseQueryString((new Uri(currUrl)).Query).Get("productId"));
                IdNotFound(!Product.NewProduct(productId));
            }
            catch (Exception ex)
            {
                IdNotFound();
                //throw;
            }
        }
    }
    public static class Product
    {
        //Id, Name, Price, Currency, Description, Image, URL
        static List<object> Elems = new List<object> { new UInt16(), String.Empty,
            new Double(), String.Empty, String.Empty, String.Empty, String.Empty };
        static string pattern = @"<a class=""kartochka-tabs__box__branding"".*<div class=""kartochka-tabs__box"" id=""box-features"">";

        public static string Pattern { get => pattern; set => pattern = value; }

        //getting product with current Id
        public static bool NewProduct(ushort id)
        {
            bool res = false;
            using (Connection newCon = new Connection("SELECT * FROM Products WHERE Id = " + id))
            {
                using (SqlDataReader reader = newCon.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int count = Elems.Count;
                        for (int i = 0; i < count; i++)
                        {
                            Elems[i] = reader.GetValue(i);
                        }
                        //Description and image aren't initialized yet
                        if (Elems[4] == DBNull.Value || Elems[5] == DBNull.Value)
                        {
                            Pair pair = GetProductMoreInfo((string)Elems[6]);
                            Elems[4] = ((string)pair.First).Replace("'", "''");
                            Elems[5] = (string)pair.Second;
                            newCon.ChangeCommandText("UPDATE Products SET Description = N'" +
                                 Elems[4] + "', Image = '" + Elems[5] + "' WHERE Id = " + id);
                            reader.Close();
                            newCon.ExecuteReader().Close();
                        }
                        res = true;
                    }
                    reader.Close();
                }
                newCon.Dispose();
            }
            return res;
        }
        public static Pair GetProductMoreInfo(string url)
        {
            string htmlCode = _Default.GetHtml(url);
            //Get description
            Regex regexDesc = new Regex(Pattern, RegexOptions.Singleline);
            string matchDesc = regexDesc.Match(htmlCode).Value;
            string[] split = matchDesc.Split('<', '>');
            int length = split.Length;

            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < length; i += 2)
            {
                if (split[i] != "")
                {
                    if (split[i - 1] == "b")
                        SB.Append("<br><b>");
                    SB.Append(split[i] + " ");
                    if (split[i - 1] == "b")
                        SB.Append("</b><br>");
                }
            }
            //Get image
            Regex regexImage = new Regex(@"image"" content="".*""/>");
            string urlImage = regexImage.Match(htmlCode).Value;
            string pathImage = "";
            if (urlImage != "" && urlImage != null)
            {
                urlImage = urlImage.Substring(16, urlImage.Length - 19);
                regexImage = new Regex(@"\w+\.jpeg");
                pathImage = "Images\\" + regexImage.Match(urlImage).Value;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(urlImage, pathImage);
                }
            }
            return new Pair(SB.ToString(), pathImage);
        }
        public static object GetElement(ushort ind)
        {
            if (ind < Elems.Count)
                return Elems[ind];
            return null;
        }
    }
}