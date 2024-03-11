using SciChatProject.Models;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using static SciChatProject.DataBaseHelper;

namespace SciChatProject
{
    public static class DataBaseHelper
    {
        public static string ConnectionString = Constants.CONNECTIONSTRING;
        
        static public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        #region READER
        public static List<T?> GetObjectsByQuery<T>(string query)
        {
            return GetValuesByQuery(query).Select(x => (T?)GetObjectByValue(x, typeof(T))).ToList();
        }

        public static List<T> GetObjects<T>() where T : SQLClass
        {
            return GetObjectsByQuery<T>($"SELECT * FROM {typeof(T).GetProperty("TableName").GetValue(null)}") as List<T>;
        }

        private static List<Dictionary<string, dynamic>> GetValuesByQuery(string query)
        {
            var result = new List<Dictionary<string, dynamic>>();

            var conn = CreateConnection();
            conn.Open();

            var cmd = new SqlCommand(query, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                var inBetweenResult = new Dictionary<string, dynamic>();
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    inBetweenResult.Add(reader.GetName(i), reader.GetValue(i)??string.Empty);
                }
                result.Add(inBetweenResult);
            }

            conn.Close();
            return result;
        }

        private static object? GetObjectByValue(Dictionary<string, dynamic> attributePairs, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach(var attributePair in attributePairs)
            {
                var property = type.GetProperties().ToList().FirstOrDefault(x=>(x.GetCustomAttribute(typeof(SQLProperty)) as SQLProperty)?.Name==attributePair.Key) ?? type.GetProperty(attributePair.Key) ?? throw new Exception($"Property for {type} not in DataBase ");
                property.SetValue(obj, attributePair.Value, null);
            }

            return obj;
        }

        #endregion READER

        #region FILLER
        public static void ExecuteChange<T>(string dataBaseName, List<T> objects, ChangeType changeType = ChangeType.Insert)
        {
            var query = CreateQueryForChange(dataBaseName, objects, changeType);
            var conn = CreateConnection();

            conn.Open();
            var cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            
            conn.Close();
        }
        private static string CreateQueryForChange<T>(string dataBaseName, List<T> objects, ChangeType changeType=ChangeType.Insert)
        {
            string result = string.Empty;

            switch (changeType)
            {
                case ChangeType.Insert:
                     result =
                        $"INSERT INTO {dataBaseName} " +
                        objects.Select(x => $"({string.Join(",", GetListOfPropertieNames(x))})").First().ToString() + // column names
                        " VALUES " +
                        string.Join(", ", objects.Select(x => $"({string.Join(",", GetListOfPropertieValues(x))})").First().ToString()); // column values
                    return result;

                case ChangeType.Update:
                    throw new NotImplementedException($"{ChangeType.Update} has not been implemented yet!");
            }

            throw new Exception("For some reason No Query String was Created!");
        }

        public static void DeleteRowFormDB<T>(string dataBaseName, int userid, int convid)
        {
            var query = $"DELETE FROM {dataBaseName} WHERE conversationid = {convid} and userid = {userid}";
            var conn = CreateConnection();

            conn.Open();
            var cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        private static List<PropertyInfo> GetListOfChangableProperties(object obj)
        {
            return obj.GetType().GetProperties().Where(x => x.GetCustomAttribute(typeof(SQLProperty)) != null).ToList();
        }

        private static List<string> GetListOfPropertieNames(object obj)
        {
            return GetListOfChangableProperties(obj).Select(x => (x.GetCustomAttribute(typeof(SQLProperty)) as SQLProperty)?.Name ?? x.Name).ToList();
        }

        private static List<string?> GetListOfPropertieValues(object obj)
        {
            var result = GetListOfChangableProperties(obj).Select(x => ModifyProperty(x.GetValue(obj))).ToList();
            return result;
        }

        private static string? ModifyProperty(dynamic? input)
        {
            if(input is string) return $"'{input}'";
            return input?.ToString();
        }

        public enum ChangeType
        {
            Update,
            Insert
        }
        #endregion FILLER
    }
}