using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Z.EntityFramework.Plus
{
    public class QueryFutureOracleDbReader : DbDataReader
    {
        public QueryFutureOracleDbReader(DbDataReader reader)
        {
            Reader = reader;
        }

        public DbDataReader Reader;

        public override int Depth
        {
            get { return Reader.Depth; }
        }

        public override bool IsClosed
        {
            get { return Reader.IsClosed; }
        }

        public override int RecordsAffected
        {
            get { return Reader.RecordsAffected; }
        }

        public override int FieldCount
        {
            get { return Reader.FieldCount; }
        }

        public override object this[int ordinal]
        {
            get { return Reader[ordinal]; }
        }

        public override object this[string name]
        {
            get { return Reader[name]; }
        }

        public override bool HasRows
        {
            get { return Reader.HasRows; }
        }

#if !NETSTANDARD1_3
        public override void Close()
        {
            Reader.Close();
        }

        public override DataTable GetSchemaTable()
        {
            return Reader.GetSchemaTable();
        }
#endif

        public override bool NextResult()
        {
            return Reader.NextResult();
        }

        public override bool Read()
        {
            return Reader.Read();
        }

        public override bool GetBoolean(int ordinal)
        {
            return Reader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return Reader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return Reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return Reader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return Reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override Guid GetGuid(int ordinal)
        {
            var value = Reader.GetValue(ordinal);
            return new Guid((byte[]) value);

            // return reader2.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return Reader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return Reader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return Reader.GetInt64(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return Reader.GetDateTime(ordinal);
        }

        public override string GetString(int ordinal)
        {
            return Reader.GetString(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return Reader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return Reader.GetDouble(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return Reader.GetFloat(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return Reader.GetName(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return Reader.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            return Reader.IsDBNull(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return Reader.GetOrdinal(name);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return Reader.GetDataTypeName(ordinal);
        }

        public override Type GetFieldType(int ordinal)
        {
            return Reader.GetFieldType(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            var value = Reader.GetValue(ordinal);
            if ((value.GetType() == typeof(byte[])) && (((byte[]) value).Length == 16))
            {
                return new Guid((byte[]) value);
            }
            return value;
        }

        public override IEnumerator GetEnumerator()
        {
            return Reader.GetEnumerator();
        }
    }
}