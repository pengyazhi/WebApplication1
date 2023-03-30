using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using WebApplication1.Models;
using WebApplication1.Controllers;
using System.Runtime.Remoting.Messaging;
using System.Web.Script.Serialization;

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
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public object IsMembershipExist(string email, string password)
        {
            var queryName = "SELECT name,id FROM Membership WHERE email = @email AND password = @password";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(queryName, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var name = (string)reader["name"];
                        var id = (int)reader["id"];
                        return Tuple.Create(name, id);

                    }
                    else
                    {
                        return null;
                    }
                }
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


        public Tuple<string,string,string> IfMembershipChanged(string name, string email, string password, string id)
        {
            var query = "UPDATE Membership SET name = @name, password = @password, email = @email WHERE id = @id";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();

                var queryShow = "SELECT name, email, password FROM Membership WHERE id = @id";
                using (var commandShow = new SqlCommand(queryShow, connection))
                {
                    string updateName = "";
                    string updateEmail = "";
                    string updatePassword = "";
                    commandShow.Parameters.AddWithValue("@id", id);
                    
                    using (var reader = commandShow.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            updateName = (string)reader["name"];
                            updateEmail = (string)reader["email"];
                            updatePassword = (string)reader["password"];
                           
                            return Tuple.Create(updateName, updateEmail, updatePassword);

                        }
                    }

                }
            return null;
            } 
        }
    }
}