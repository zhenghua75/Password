using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Data;

namespace Decrypt
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            MembershipProvider provider = Membership.Provider;
            List<User> users = new List<User>();
            using(SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["FairiesMemberManage"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserId,UserName FROM aspnet_Users", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            users.Add(new User()
                            {
                                UserId = reader.GetGuid(0),
                                UserName = reader.GetString(1)
                            });
                        }
                        
                    }
                }
                foreach (var user in users)
                {
                    cmd = new SqlCommand("UPDATE aspnet_Users SET PasswordDecrypt=@PasswordDecrypt WHERE UserId=@UserId", conn);
                    SqlParameter para = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
                    para.Value = user.UserId;
                    cmd.Parameters.Add(para);

                    para = new SqlParameter("@PasswordDecrypt", SqlDbType.VarChar);
                    para.Value = provider.GetPassword(user.UserName, "");
                    cmd.Parameters.Add(para);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("完成");
            Console.Read();
        }
    }
}
