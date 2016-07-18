using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

using SpotDao;
using TableDAO;
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
                switch (ex.Number){
                    case 0:
                        throw new Exception("Cannot connect to server.  Contact administrator");

                    case 1045:
                        throw new Exception("Invalid username/password, please try again");
                }
            }
        }

        //Close connection to database
        private void CloseConnection(){
            try{
                connection.Close();
            }
            catch (MySqlException){
                throw new Exception("Error to close connection with db");
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
                throw e;
            }

        }

        public void readSpots(List<Spot> spots, int studyroomId) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT id, x, y, is_free, count FROM spot WHERE room_id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", studyroomId);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        int id = int.Parse(dataReader.GetString(0));
                        int x = int.Parse(dataReader.GetString(1));
                        int y = int.Parse(dataReader.GetString(2));
                        bool isFree = int.Parse(dataReader.GetString(3)) == 0 ? false : true;
                        int count = int.Parse(dataReader.GetString(4));
                        spots.Add(new Spot(id, x, y, isFree, count));
                    }
                    dataReader.Close();
                    CloseConnection();
                } else {
                    CloseConnection();
                }
            } catch (Exception e) {
                throw e;
            }
        }

        public void addSpot(Spot spot, int studyroomId) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO spot(x, y, is_free, room_id, count) VALUES(@x, @y, @isFree, @idRoom, @count)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@x", spot.X);
                cmd.Parameters.AddWithValue("@y", spot.Y);
                cmd.Parameters.AddWithValue("@isFree", spot.IsFree);
                cmd.Parameters.AddWithValue("@idRoom", studyroomId);
                cmd.Parameters.AddWithValue("@count", spot.Count);
                cmd.ExecuteNonQuery();
                Console.WriteLine("x, y, isFree, idRoom" + spot.X + " " + spot.Y + " " + spot.IsFree + " " + studyroomId);
                CloseConnection();
            } catch (Exception) {
            }
        }

        public void updateSpot(Spot spot) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE spot SET is_free=@isFree, count=@count WHERE id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", spot.Id);
                cmd.Parameters.AddWithValue("@isFree", spot.IsFree);
                cmd.Parameters.AddWithValue("@count", spot.Count);
                cmd.ExecuteNonQuery();
                CloseConnection();
            } catch (Exception) {
            }
        }

        public void readTable(List<Table> tables, int studyroomId) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT id, x_TopL, y_TopL, height, width FROM room_table WHERE room_id=@roomId";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@roomId", studyroomId);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        int id = int.Parse(dataReader.GetString(0));
                        int x = int.Parse(dataReader.GetString(1));
                        int y = int.Parse(dataReader.GetString(2));
                        int height = int.Parse(dataReader.GetString(3));
                        int width = int.Parse(dataReader.GetString(4));

                        tables.Add(new Table(id, x, y, height, width));
                    }
                    dataReader.Close();
                    CloseConnection();
                } else {
                    CloseConnection();
                }
            } catch (Exception e) {
                throw e;
            }
        }

        public void addTable(Table table, int studyroomId) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO room_table(x_TopL, y_TopL, height, width, room_id) VALUES(@x, @y, @height, @width, @room_id)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@x", table.X);
                cmd.Parameters.AddWithValue("@y", table.Y);
                cmd.Parameters.AddWithValue("@height", table.Height);
                cmd.Parameters.AddWithValue("@width", table.Width);
                cmd.Parameters.AddWithValue("@room_id", studyroomId);
                cmd.ExecuteNonQuery();
                CloseConnection();
            } catch (Exception) {
            }
        }

        public String getRoom0(int studyroomId) {
            try {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT room0_path FROM studyroom WHERE id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", studyroomId);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows) {
                    if (dataReader.Read()) {
                        String room0 = dataReader.GetString(0);
                        CloseConnection();
                        return room0;
                    }
                    dataReader.Close();
                    CloseConnection();
                    throw new DatabaseException();
                } else {
                    CloseConnection();
                    throw new DatabaseException();
                }
            } catch (Exception e) {
                throw e;
            }
        }

        public Boolean isValidToken(int id, String token)
        {
            try
            {
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT token FROM studyroom WHERE id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    String tokenValid = null;
                    if (dataReader.Read())
                        tokenValid = dataReader.GetString(0);
                    dataReader.Close();
                    CloseConnection();
                    if (tokenValid.Equals(token))
                        return true;
                    else
                        return false;
                }
                else
                {
                    CloseConnection();
                    throw new Exception("");
                }
            }
            catch (Exception e)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    CloseConnection();
                throw new DatabaseException();
            }
        }

        public Boolean provaToken(String token) {
            try {
                    OpenConnection();
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = connection;
                    cmd2.CommandText = "UPDATE studyroom SET token=@token WHERE owner_user=@user";
                    cmd2.Prepare();
                    cmd2.Parameters.AddWithValue("@token", token);
                    cmd2.Parameters.AddWithValue("@user", "Davide");
                    cmd2.ExecuteNonQuery();
                    CloseConnection();
                    return true;
            } catch (Exception e) {
                if (connection.State == System.Data.ConnectionState.Open)
                    CloseConnection();
                throw new DatabaseException();
            }
        }

        public State getState(int id) {
            try {
                State state;
                OpenConnection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT state FROM studyroom WHERE id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows) {
                    String phase = null;
                    if(dataReader.Read())
                        phase = dataReader.GetString(0);
                    dataReader.Close();
                    CloseConnection();
                    if(phase.Equals("Learning"))
                        state = State.Learning;
                    else if(phase.Equals("Monitoring"))
                        state = State.Monitoring;
                    else
                        throw new Exception("");
                    return state;
                } else {
                    CloseConnection();
                    throw new Exception("");
                }
            } catch (Exception e) {
                throw e;
            }
        }

    }
}
