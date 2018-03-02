using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypt
{
    public class User
    {
        public Guid UserId { get; set; }
        public string PasswordEncrypt { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FairiesMemberManage"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserId,PasswordDecrypt FROM aspnet_Users", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string passwordDecrypt = reader.GetString(1);
                            PasswordHasher hasher = new PasswordHasher();
                            string passwordEncrypt = hasher.HashPassword(passwordDecrypt);
                            users.Add(new User()
                            {
                                UserId = reader.GetGuid(0),
                                PasswordEncrypt = passwordEncrypt
                            });
                        }

                    }
                }
                foreach (var user in users)
                {
                    cmd = new SqlCommand("UPDATE aspnet_Users SET PasswordEncrypt=@PasswordEncrypt WHERE UserId=@UserId", conn);
                    SqlParameter para = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
                    para.Value = user.UserId;
                    cmd.Parameters.Add(para);

                    para = new SqlParameter("@PasswordEncrypt", SqlDbType.VarChar);
                    para.Value = user.PasswordEncrypt;
                    cmd.Parameters.Add(para);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("完成");
            Console.Read();
        }
    }
}
