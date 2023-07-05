using MySql.Data.MySqlClient;
using System.Data;

namespace API.Helpers
{
    public static class Common
    {
        public static string SQLCONSTRING = "";
        public static string SSERVER = "127.0.0.1";
        public static string SDATABASE = "Lottery";
        public static string SUSERNAME = "root";
        public static string SPASSWORD = "123";
        public static string SPORT = "3306";
        public static string SPORTSTATE = "1";

        public static string FK_OpenDB()
        {
            if (SPORTSTATE == "1")
            {
                SQLCONSTRING = "server='" + SSERVER + "';port='" + SPORT + "';database='" + SDATABASE + "';uid='" + SUSERNAME + "';pwd='" + SPASSWORD + "';Convert Zero Datetime=True;";
            }
            else
            {
                SQLCONSTRING = "server='" + SSERVER + "';database='" + SDATABASE + "';uid='" + SUSERNAME + "';pwd='" + SPASSWORD + "';Convert Zero Datetime=True;";
            }

            using (MySqlConnection sqlcon = new MySqlConnection(SQLCONSTRING))
            {
                var result = "";
                try
                {
                    if (sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                        return "Ok";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return ex.Message;
                }

                return result;
            }

            
        }

    }
}
