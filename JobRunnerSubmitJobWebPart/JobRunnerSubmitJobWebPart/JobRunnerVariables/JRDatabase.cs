using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace JobRunnerSubmitJobWebPart.JobRunnerVariables
{
    class JRDatabase
    {
        public Boolean AddToFileQueue( string _CmdFile, int _ExecGroupId, int _JobId )
        {
            Boolean OK = false;
            string sqlserver = "schooner";
            string connectionstring = "Data Source=" + sqlserver + "; Initial Catalog=JobRunner;Integrated Security=sspi";

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("pEnqueueFile", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CmdFile", _CmdFile));
                    cmd.Parameters.Add(new SqlParameter("@ExecGroupId", _ExecGroupId));
                    cmd.Parameters.Add(new SqlParameter("@JobId", _JobId));
                    cmd.ExecuteNonQuery();
                    OK = true;
                }
                catch (SqlException ex)
                {
                    //TODO: handle exception
                }
                return OK;
            }
        }


        public int GetNewJobId()
        {
            string sqlserver = "schooner";
            string connectionstring = "Data Source=" + sqlserver + "; Initial Catalog=JobRunner;Integrated Security=sspi";
            
            int JobId = 1;
            
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("pGetNewJobId", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    Object sql_result = cmd.ExecuteScalar();
                    if (sql_result == DBNull.Value)
                        return 1;
                    JobId = Convert.ToInt32(sql_result);
                }
                catch (SqlException ex)
                {
                    //TODO: handle exception
                }
                return JobId;
            }
        }
    }
}
