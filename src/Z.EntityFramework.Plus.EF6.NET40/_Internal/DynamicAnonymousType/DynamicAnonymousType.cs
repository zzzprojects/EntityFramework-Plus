// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Z.EntityFramework.Plus
{
    internal static class DynamicAnonymousType
    {

        private static readonly AssemblyName AssemblyName = new AssemblyName { Name = "<>f__AnonymousType" };
#if NETSTANDARD1_3
        private static readonly ModuleBuilder ModuleBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
#else
        private static readonly ModuleBuilder ModuleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
#endif
        private static readonly Dictionary<string, Tuple<string, Type>> BuiltTypes = new Dictionary<string, Tuple<string, Type>>();

        public static object Create(List<Tuple<string, object>> values)
        {
            if (values == null || values.Count == 0)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            var types = new List<Tuple<string, Type>>();

            foreach (var value in values)
            {
                types.Add(new Tuple<string, Type>(value.Item1, value.Item2.GetType()));
            }

            var type = GetDynamicType(types);
            var obj = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

            foreach (var value in values)
            {
                var fieldInfo = type.GetField(value.Item1);
                fieldInfo.SetValue(obj, value.Item2);
            }

            return obj;
        }

        public static Expression CreateExpression(List<Tuple<string, Expression>> values)
        {
            if (values == null || values.Count == 0)
            {
                throw new Exception(ExceptionMessage.GeneralException);
            }

            var types = new List<Tuple<string, Type>>();
            var memberInits = new List<MemberAssignment>();

            foreach (var value in values)
            {
                types.Add(new Tuple<string, Type>(value.Item1, value.Item2.Type));
            }

            var type = GetDynamicType(types);


            var constructorInfo = type.GetConstructor(Type.EmptyTypes);

            foreach (var value in values)
            {
                memberInits.Add(Expression.Bind(type.GetField(value.Item1), value.Item2));
            }

            return Expression.MemberInit(Expression.New(constructorInfo), memberInits);
        }

        private static string GetTypeKey(List<Tuple<string, Type>> fields)
        {
            var sb = new StringBuilder();

            foreach (var field in fields)
            {
                sb.Append(field.Item1);
                sb.Append(";");
                sb.Append(field.Item2.Name);
                sb.Append(";");
            }

            return sb.ToString();
        }

        public static Type GetDynamicType(List<Tuple<string, Type>> fields)
        {
            try
            {
                Monitor.Enter(BuiltTypes);
                var typeKey = GetTypeKey(fields);

                if (BuiltTypes.ContainsKey(typeKey))
                    return BuiltTypes[typeKey].Item2;

                var typeName = "<>f__AnonymousType" + BuiltTypes.Count;
                var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, null, Type.EmptyTypes);

                foreach (var field in fields)
                    typeBuilder.DefineField(field.Item1, field.Item2, FieldAttributes.Public);

#if NETSTANDARD1_3
                BuiltTypes[typeKey] = new Tuple<string, Type>(typeName, typeBuilder.CreateTypeInfo().AsType());
#else
                BuiltTypes[typeKey] = new Tuple<string, Type>(typeName, typeBuilder.CreateType());
#endif

                return BuiltTypes[typeKey].Item2;
            }
            finally
            {
                Monitor.Exit(BuiltTypes);
            }
        }
    }
}

#endif