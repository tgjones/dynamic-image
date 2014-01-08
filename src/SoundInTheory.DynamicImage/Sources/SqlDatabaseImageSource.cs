using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Sources
{
	public class SqlDatabaseImageSource : ImageSource
	{
		#region Properties

		public string ConnectionStringName
		{
			get { return (string)(this["ConnectionStringName"] ?? string.Empty); }
			set { this["ConnectionStringName"] = value; }
		}

		public string TableName
		{
			get { return (string)(this["TableName"] ?? string.Empty); }
			set { this["TableName"] = value; }
		}

		[Category("Data"), DefaultValue("")]
		public string ColumnName
		{
			get { return (string)(this["ColumnName"] ?? string.Empty); }
			set { this["ColumnName"] = value; }
		}

		public string[] PrimaryKeyNames
		{
			get { return (string[]) this["PrimaryKeyNames"]; }
			set { this["PrimaryKeyNames"] = value; }
		}

		public string PrimaryKeyName
		{
			get { return (PrimaryKeyNames != null && PrimaryKeyNames.Length > 0) ? PrimaryKeyNames[0] : string.Empty; }
			set { PrimaryKeyNames = new[] { value }; }
		}

		public object[] PrimaryKeyValues
		{
			get { return (object[]) this["PrimaryKeyValues"]; }
			set { this["PrimaryKeyValues"] = value; }
		}

		public object PrimaryKeyValue
		{
			get { return (PrimaryKeyValues != null && PrimaryKeyValues.Length > 0) ? PrimaryKeyValues[0] : null; }
			set { PrimaryKeyValues = new[] { value }; }
		}

		#endregion

        public override FastBitmap GetBitmap()
		{
			SqlConnection conn = null; SqlDataAdapter adapter = null; DataTable dt = null;
			try
			{
				string connectionString = ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString;
				conn = new SqlConnection(connectionString);
				conn.Open();

				string whereFilter = string.Empty;
				for (int i = 0, length = PrimaryKeyNames.Length; i < length; ++i)
				{
					string primaryKeyName = PrimaryKeyNames[i];
					object primaryKeyValue = PrimaryKeyValues[i];
					whereFilter += string.Format("{0} = {1}", primaryKeyName, primaryKeyValue);
					if (i < length - 1)
						whereFilter += " AND ";
				}

				string selectSql = string.Format("SELECT {0} FROM {1} WHERE {2}",
					ColumnName, TableName, whereFilter);

				adapter = new SqlDataAdapter(selectSql, conn);

				dt = new DataTable();
				adapter.Fill(dt);

				if (dt.Rows.Count != 1)
					throw new InvalidOperationException("Could not retrieve value from database");

				object value = dt.Rows[0][0];

				if (value == null || value is DBNull)
					return null; // this is fine, just means there's no value in the database

				if (!(value is byte[]))
					throw new InvalidOperationException(string.Format("Expected object of type '{0}' but found object of type '{1}'", typeof(byte[]).FullName, value.GetType().FullName));

				return new FastBitmap((byte[]) value);
			}
			finally
			{
				if (dt != null)
					dt.Dispose();
				if (adapter != null)
					adapter.Dispose();
				if (conn != null)
				{
					conn.Close();
					conn.Dispose();
				}
			}
		}

		public override void PopulateDependencies(List<Dependency> dependencies)
		{
			Dependency dependency = new Dependency();
			dependency.Text1 = ConnectionStringName;
			dependency.Text2 = TableName;
			dependency.Text3 = (PrimaryKeyValues != null) ? string.Join(",", PrimaryKeyValues) : string.Empty;
			dependencies.Add(dependency);
		}
	}
}
