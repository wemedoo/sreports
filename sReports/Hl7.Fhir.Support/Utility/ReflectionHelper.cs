/* 
 * Copyright (c) 2014, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/FirelyTeam/fhir-net-api/master/LICENSE
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hl7.Fhir.Utility
{
    public static class ReflectionHelper
    {
        public static bool IsAValueType(this Type t)
        {
            return t.GetTypeInfo().IsValueType;
        }

        public static bool CanBeTreatedAsType(this Type currentType, Type typeToCompareWith)
        {
            // Always return false if either Type is null
            if (currentType == null || typeToCompareWith == null)
                return false;

            // Return the result of the assignability test
            return typeToCompareWith.GetTypeInfo().IsAssignableFrom(currentType.GetTypeInfo());
        }


        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOnEnum<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetTypeInfo().GetDeclaredField(enumVal.ToString());
            var attributes = memInfo.GetCustomAttributes(typeof(T), false);
            return (attributes.Count() > 0) ? (T)attributes.First() : null;
        }


        public static IEnumerable<PropertyInfo> FindPublicProperties(Type t)
        {
            if (t == null) throw Error.ArgumentNull("t");

#if NETSTANDARD1_1
            // Unfortunately, netstandard1.0 has no method to filter on bindingflags :-(
            // Have to do it ourselves
            return t.GetRuntimeProperties().Where(p => hasPublicInstanceReadAccessor(p));
            //return t.GetRuntimeProperties(); //(BindingFlags.Instance | BindingFlags.Public);
            // return t.GetTypeInfo().DeclaredProperties.Union(t.GetTypeInfo().BaseType.GetTypeInfo().DeclaredProperties); //(BindingFlags.Instance | BindingFlags.Public);

            bool hasPublicInstanceReadAccessor(PropertyInfo p)
            {
                if (!p.CanRead) return false;
                var getMethod = p.GetMethod;
                return getMethod.IsPublic && !getMethod.IsStatic;
            }
#else
            return t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
#endif
        }

//        public static PropertyInfo FindPublicProperty(Type t, string name)
//        {
//            if (t == null) throw Error.ArgumentNull("t");
//            if (name == null) throw Error.ArgumentNull("name");

//            return t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
//        }

//        internal static MethodInfo FindPublicStaticMethod(Type t, string name, params Type[] arguments)
//        {
//            if (t == null) throw Error.ArgumentNull("t");
//            if (name == null) throw Error.ArgumentNull("name");

//            return t.GetMethod(name, arguments);
//        }

        public static bool HasDefaultPublicConstructor(Type t)
        {
            if (t == null) throw Error.ArgumentNull("t");

            if (t.GetTypeInfo().IsValueType)
                return true;

            return (GetDefaultPublicConstructor(t) != null);
        }

        internal static ConstructorInfo GetDefaultPublicConstructor(Type t)
        {
#if NETSTANDARD1_1
            return t.GetTypeInfo().DeclaredConstructors.FirstOrDefault(s => s.GetParameters().Length == 0 && s.IsPublic && !s.IsStatic);
#else
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            return t.GetConstructors(bindingFlags).SingleOrDefault(c => !c.GetParameters().Any());
#endif
        }

        public static bool IsNullableType(Type type)
        {
            if (type == null) throw Error.ArgumentNull("type");

            return (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetNullableArgument(Type type)
        {
            if (type == null) throw Error.ArgumentNull("type");

            if (IsNullableType(type))
            {
                return type.GetTypeInfo().GenericTypeArguments[0];
            }
            else
                throw Error.Argument("type", "Type {0} is not a Nullable<T>".FormatWith(type.Name));
        }

        public static bool IsTypedCollection(Type type)
        {
            return type.IsArray || ImplementsGenericDefinition(type, typeof(ICollection<>));
        }


        public static IList CreateGenericList(Type itemType)
        {
            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
        }


        public static bool IsClosedGenericType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().ContainsGenericParameters;
        }


        public static bool IsOpenGenericTypeDefinition(Type type)
        {
           return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        public static bool IsConstructedFromGenericTypeDefinition(Type type, Type genericBase)
        {
            return type.GetGenericTypeDefinition() == genericBase;
        }

        /// <summary>
        /// Gets the type of the typed collection's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed collection's items.</returns>
        public static Type GetCollectionItemType(Type type)
        {
            if (type == null) throw Error.ArgumentNull("type");

            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (ImplementsGenericDefinition(type, typeof(ICollection<>), out Type genericListType))
            {
                //EK: If I look at ImplementsGenericDefinition, I don't think this can actually occur.
                //if (genericListType.IsGenericTypeDefinition)
                //throw Error.Argument("type", "Type {0} is not a collection.", type.Name);

                return genericListType.GetTypeInfo().GenericTypeArguments[0];
            }
            else if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return null;
            }
            else
            {
                throw Error.Argument("type", "Type {0} is not a collection.".FormatWith(type.Name));
            }
        }
        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition) =>
            ImplementsGenericDefinition(type, genericInterfaceDefinition, out Type implementingType);

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            if (type == null) throw Error.ArgumentNull("type");
            if (genericInterfaceDefinition == null) throw Error.ArgumentNull("genericInterfaceDefinition");

            if (!genericInterfaceDefinition.GetTypeInfo().IsInterface || !genericInterfaceDefinition.GetTypeInfo().IsGenericTypeDefinition)
                throw Error.Argument("genericInterfaceDefinition", "'{0}' is not a generic interface definition.".FormatWith(genericInterfaceDefinition.Name));

           if (type.GetTypeInfo().IsInterface)
            {
                if (type.GetTypeInfo().IsGenericType)
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (Type i in type.GetTypeInfo().ImplementedInterfaces)
            {
                if (i.GetTypeInfo().IsGenericType)
                {
                    Type interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = i;
                        return true;
                    }
                }
            }

            implementingType = null;
            return false;
        }

        #region << Extension methods to make the handling of PCL easier >>

       public static bool IsEnum(this Type t)
        {
			return t.GetTypeInfo().IsEnum;
        }
        #endregion

        public static T GetAttribute<T>(MemberInfo member) where T : Attribute
        {
            return member.GetCustomAttribute<T>();
        }

        internal static ICollection<T> GetAttributes<T>(MemberInfo member) where T : Attribute
        {
            var attr = member.GetCustomAttributes<T>();
            return (ICollection<T>)attr.Cast<T>();
        }


        internal static IEnumerable<FieldInfo> FindEnumFields(Type t)
        {
            if (t == null) throw Error.ArgumentNull("t");

            return t.GetTypeInfo().DeclaredFields.Where(a => a.IsPublic && a.IsStatic);
        }

        public static bool IsArray(object value)
        {
            if (value == null) throw Error.ArgumentNull("value");

            return value.GetType().IsArray;
        }

        public static string PrettyTypeName(Type t)
        {
            // http://stackoverflow.com/questions/1533115/get-generictype-name-in-good-format-using-reflection-on-c-sharp#answer-25287378 
            return t.GetTypeInfo().IsGenericType ? string.Format( 
                "{0}<{1}>", t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.CurrentCulture)), 
                string.Join(", ", t.GetTypeInfo().GenericTypeParameters.ToList().Select(PrettyTypeName))) 
            : t.Name;
        }
    }
}
