using System.Reflection;
using UnityEngine;

public static class MemberInfo_Extension
{
    /*
    public static object GetValue(this MemberInfo memberInfo, object forObject)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).GetValue(forObject);
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).GetValue(forObject);
            default:
                Debug.LogWarning(
                    $"the member {memberInfo.Name} is neither a Field, nor a Property, " +
                    $"so it isn't recognized by {nameof(GetValue)}. Returning null by default.");
                return null;
        }
    }
    */

}
