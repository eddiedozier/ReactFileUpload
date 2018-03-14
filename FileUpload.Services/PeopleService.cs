using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using FileUpload.Models.Request;
using FileUpload.Models.Domain;

namespace FileUpload.Services
{
    public class PeopleService
    {
        public IConfigurationRoot Configuration { get; }
        public string connectionString;

        public PeopleService()
        {
           var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables();
            Configuration = builder.Build();

            connectionString = Configuration.GetConnectionString("DefaultConnection");
        }


        public int Insert(PeopleAddRequest model)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string cmdText = "Eddie_Dozier_Insert";
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter();
                    parm.ParameterName = "@Id";
                    parm.SqlDbType = System.Data.SqlDbType.Int;
                    parm.Direction = System.Data.ParameterDirection.Output;

                    cmd.Parameters.Add(parm);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@MiddleInitial", model.MiddleInitial);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                    cmd.Parameters.AddWithValue("@ModifiedBy", model.ModifiedBy);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    result = (int)cmd.Parameters["@Id"].Value;
                    conn.Close();
                }
            }
            return result;
        }

        public List<People> GetAll()
        {
            List<People> result = new List<People>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string cmdText = "Eddie_Dozier_SelectAll";
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        People model = new People();
                        int index = 0;
                        model.Id = reader.GetInt32(index++);
                        model.FirstName = reader.GetString(index++);
                        if (!reader.IsDBNull(index))
                            model.MiddleInitial = reader.GetString(index++)[0];
                        model.LastName = reader.GetString(index++);
                        model.DOB = reader.GetDateTime(index++);
                        model.CreatedDate = reader.GetDateTime(index++);
                        model.ModifiedDate = reader.GetDateTime(index++);
                        model.ModifiedBy = reader.GetString(index++);
                        result.Add(model);
                    }
                    conn.Close();
                }
            }
            return result;
        }
    }
}