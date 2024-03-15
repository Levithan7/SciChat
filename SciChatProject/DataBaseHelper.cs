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
            return GetObjectsByQuery<T>($"SELECT * FROM {typeof(T)?.GetProperty("TableName")?.GetValue(null)}") as List<T>;
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
            var query = CreateQueryForChange(dataBaseName, objects, out var parameters, changeType);
            var conn = CreateConnection();

            conn.Open();
            var cmd = new SqlCommand(query, conn);
            foreach(var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
            cmd.ExecuteNonQuery();
            
            conn.Close();
        }

        private static List<string> GetCommandParameters(object x, List<KeyValuePair<string, string>> parameters)
        {
            int counter=0;
            parameters.AddRange(GetListOfPropertieValues(x).Select(y=>new KeyValuePair<string, string>($"@ARG{counter++}", y)).ToList());
            return parameters.Select(x => x.Key).ToList();
		}

		private static string CreateQueryForChange<T>(string dataBaseName, List<T> objects, out List<KeyValuePair<string, string>> parameters, ChangeType changeType = ChangeType.Insert)
        {
			List<KeyValuePair<string, string>> parametersUpdated = new();
            parameters = new();
			string query = string.Empty;
			switch (changeType)
			{
				case ChangeType.Insert:
					query =
					   $"INSERT INTO {dataBaseName} " +
					   objects.Select(x => $"({string.Join(",", GetListOfPropertieNames(x))})").First().ToString() + // column names
					   " VALUES " +
					   string.Join(", ", objects.Select(x => $"({string.Join(",", GetCommandParameters(x, parametersUpdated))})").First().ToString()); // column values
					parameters = parametersUpdated;
					return query;

				// not suitable for anythin with '' in it
                case ChangeType.Delete:
					foreach (var curobj in objects)
					{
						var propNames = GetListOfPropertieNames(curobj);
						query += $"DELETE FROM {dataBaseName} WHERE" + (string.Join(" ",
							propNames.Select(x => $" {x} = {GetDictOfProperties(curobj)[x]} AND")
							));
						query = query.Substring(0, query.Length - 3);
						query += "; ";
					}
					return query;
			}
            

			throw new Exception("For some reason No Query String was Created!");
		}


		private static string CreateQueryForChange<T>(string dataBaseName, List<T> objects, ChangeType changeType=ChangeType.Insert)
        {
            string query = string.Empty;
            switch (changeType)
            {
                case ChangeType.Insert:
                     query =
                        $"INSERT INTO {dataBaseName} " +
                        objects.Select(x => $"({string.Join(",", GetListOfPropertieNames(x))})").First().ToString() + // column names
                        " VALUES " +
                        string.Join(", ", objects.Select(x => $"({string.Join(",", GetListOfPropertieValues(x))})").First().ToString()); // column values
                    return query;

                case ChangeType.Delete:
                    foreach(var curobj in objects)
                    {
                        var propNames = GetListOfPropertieNames(curobj);
					    query += $"DELETE FROM {dataBaseName} WHERE" + (string.Join(" ",
                            propNames.Select(x=>$" {x} = {GetDictOfProperties(curobj)[x]} AND")
                            ));
                        query = query.Substring(0, query.Length-3);
                        query += "; ";
					}
                    return query;
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

        private static Dictionary<string, string> GetDictOfProperties(object obj)
        {
            var names = GetListOfPropertieNames(obj);
            var values = GetListOfPropertieValues(obj);
            var result = new Dictionary<string, string>();
            names.ForEach(x => result.Add(x, values[names.IndexOf(x)]));
            return result;
        }

        // OUTDATED -> not needed right now; but could be used later in case needed
        private static string? ModifyProperty(dynamic? input)
        {
            return input?.ToString();
        }

        public enum ChangeType
        {
            Update,
            Insert,
            Delete
        }
        #endregion FILLER
    }
}