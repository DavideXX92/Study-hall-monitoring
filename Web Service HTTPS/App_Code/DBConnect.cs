using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DB{
    class DBConnect{
        private MySqlConnection connection;
        private string server;
        private string database;
        private string username;
        private string password;

        public DBConnect(){
            this.server = "localhost";
            this.username = "root";
            this.password = "";
            this.database = "polistudio";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            this.connection = new MySqlConnection(connectionString);
        }

        //Open connection to database
        private void OpenConnection(){
            try{
                connection.Open();
            }
            catch (MySqlException ex){
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number){
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
            }
        }

        //Close connection to database
        private void CloseConnection(){
            try{
                connection.Close();
            }
            catch (MySqlException){
            }
        }

        public List<string> showTables(){
            List<string> tables = new List<string>();
            string query = "SHOW TABLES";
            try{
                OpenConnection();
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                while (dataReader.Read())
                    tables.Add(dataReader.GetString(0));
                //close Data Reader
                dataReader.Close();

                CloseConnection();
                return tables;
            }
            catch (Exception e){
                throw;
            }

        }

        public Boolean login(String username, String password, String token) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;              
                cmd.CommandText = "SELECT username FROM user WHERE username=@user AND hashpw=@pwd";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pwd", password);                
                MySqlDataReader dataReader = cmd.ExecuteReader();            

                if (dataReader.HasRows) {
                    CloseConnection();
                    OpenConnection();
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = connection;
                    cmd2.CommandText = "UPDATE studyroom SET token=@token WHERE owner_user=@user";
                    cmd2.Prepare();
                    cmd2.Parameters.AddWithValue("@token", token);
                    cmd2.Parameters.AddWithValue("@user", username);
                    cmd2.ExecuteNonQuery();
                    CloseConnection();
                    return true;
                } else {
                    CloseConnection();
                    return false;
                }
            } catch (Exception) {
                if (connection.State == System.Data.ConnectionState.Open)
                    CloseConnection();
                throw new DatabaseException();
            }
        }

        /*
         * It returns the id of the studyroom whose token is the method param (0 if no token matches)
         */ 
        public int isTokenValid(String token) {    
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT id FROM studyroom WHERE token=@token";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@token", token);
                MySqlDataReader dataReader = cmd.ExecuteReader();                

                if (dataReader.HasRows) {            
                    if (token == "invalid")
                        throw new DatabaseException();
                    if (dataReader.Read()) {
                        int id = int.Parse(dataReader.GetString(0));
                        CloseConnection();
                        return id;
                    }
                    CloseConnection();
                    return 0;
                } else {
                    CloseConnection();
                    return 0;
                }
            } catch (Exception) {
                if (connection.State == System.Data.ConnectionState.Open)
                    CloseConnection();
                throw new DatabaseException();
            }
        }

        public bool logout(String token) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE studyroom SET token='invalid' WHERE token=@token";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@token", token);
                cmd.ExecuteNonQuery();
                CloseConnection();
                return true;
            } catch(Exception) {
                return false;
            }
        }
    }
}
