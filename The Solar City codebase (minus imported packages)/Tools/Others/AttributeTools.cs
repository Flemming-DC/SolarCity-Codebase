using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

/*
public static class AttributeTools
{


    public static List<MemberInfo> GetStaticMembersWithAttribute<T>() where T : Attribute
    {
        List<MemberInfo> membersWithAttribute = new List<MemberInfo>();
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; // this allows GetMembers() to acces private members 

        foreach (var type in GetAllTypes())
            membersWithAttribute.AddRange(type.GetMembers(flags)
                                              .Where(member => HasAttribute<T>(member))
                                         );
        return membersWithAttribute;
    }



    public static List<Type> GetTypesWithAttribute<T>() where T : Attribute
    {
        return GetAllTypes().Where(type => HasAttribute<T>(type)).ToList();
    }


    public static List<Type> GetAllTypes()
    {
        List<Type> allTypes = new List<Type>();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
            allTypes.AddRange(assembly.GetTypes());

        return allTypes;
    }


    public static bool HasAttribute<T>(Type type) where T : Attribute
    {
        if (type.CustomAttributes.ToArray().Length > 0)
        {
            T attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
                return true;
        }
        return false;
    }

    public static bool HasAttribute<T>(MemberInfo member) where T : Attribute
    {
        if (member.CustomAttributes.ToArray().Length > 0)
        {
            T attribute = member.GetCustomAttribute<T>();
            if (attribute != null)
                return true;
        }
        return false;
    }


}
*/
