using SciChatProject.Models;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace SciChatProject
{
    public static class DataBaseHelper
    {
        public static string ConnectionString = "";
        
        static public SqlConnection OpenConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static List<T?> GetObjectsByQuery<T>(string query)
        {
            return GetValuesByCommand(query).Select(x => (T?)GetObject(x, typeof(T))).ToList();
        }

        private static List<Dictionary<string, dynamic>> GetValuesByCommand(string query)
        {
            var result = new List<Dictionary<string, dynamic>>();

            var conn = OpenConnection();
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

        private static Object? GetObject(Dictionary<string, dynamic> attributePairs, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach(var attributePair in attributePairs)
            {
                var property = type.GetProperty(attributePair.Key);
                if (property == null) continue;

                var propertyValue = attributePair.Value;

                property.SetValue(obj, propertyValue, null);
            }

            return obj;
        }

        public static string CreateQueryForChange<T>(string dataBaseName, List<T> objects, ChangeType changeType=ChangeType.Update)
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
                    throw new NotImplementedException($"{ChangeType.Update.ToString()} has not been implemented yet!");
            }

            throw new Exception("For some reason No Query String was Created!");
        }

        private static List<string> GetListOfPropertieNames(object obj)
        {
            var result = obj.GetType().GetProperties().Select(x => x.Name).ToList();
            return result;
        }


        private static List<string?> GetListOfPropertieValues(object obj)
        {
            var result = obj.GetType().GetProperties().Select(x => ModifyProperty(x.GetValue(obj))).ToList();
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
    }
}