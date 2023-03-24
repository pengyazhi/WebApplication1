using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class MembershipDB
    {
        //XmlDocument doc = new XmlDocument();
        //doc.Load("My_Web_ProjectSetting.xml");
        //XmlNode severNode = doc.SelectSingleNode("/root/config[1]/sever");
        // 獲取標籤內容
        //string sever = severNode.InnerText;
        public string connectionString { get; set; } = "";

        public readonly string computerName = System.Environment.MachineName;//電腦名稱

        public void ConnectToDatabase()
        {
            switch (computerName)
            {
                case "SIHYONG":
                    connectionString = "Data Source=SIHYONG;Initial Catalog=yazhi;User Id=yazhipeng;Password=pasta9931";
                    break;
                case "YAZHI":
                    connectionString = "Data Source=YAZHI\\SQLEXPRESS;Initial Catalog=YAZHIPENG.TEST;User Id=PENGYAZHI;Password=pasta9931";
                    break;
                default:
                    connectionString = "Data Source=YAZHI\\SQLEXPRESS;Initial Catalog=YAZHIPENG.TEST;User Id=PENGYAZHI;Password=pasta9931";
                    break;
            }
            var connection = new SqlConnection(connectionString);
        }
        public void SaveMembership(string name, string email, string password)
        {
            var query = "INSERT INTO Membership (name, email, password) VALUES (@name, @email, @password)";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
            }
        }
        public string IsMembershipExist(string email, string password)
        {
            var query = "SELECT name FROM Membership WHERE email = @email AND password = @password";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();
                /* using (var reader = command.ExecuteReader())
                 {
                     var results = new List<string>();
                     while (reader.Read())
                     {
                         var name = reader.GetString(0);
                         results.Add(name);
                     }
                     return results.ToArray();
                 }*/

                var result = (string)command.ExecuteScalar();
                return result;
            }
        }
        public bool IsEmailExist(string email)
        {
            var query = "SELECT COUNT(*) FROM Membership WHERE email = @email";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email);

                connection.Open();

                var result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }
        public void IfMembershipChanged(string name, string email, string password)
        {
            var query = "UPDATE Membership SET name = @name, password = @password,email = @email WHERE email = @email";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}