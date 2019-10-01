using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    string targetName;

    public static bool Show { get; private set; }
    /// <summary>
    ///  Shows the field if the return value is true. Include '#' at the start of the string to reference a field or property.
    /// </summary>
    /// <param name="functionFieldOrProperty"></param>
    public ShowIfAttribute(string functionFieldOrProperty)
    {
        if (string.IsNullOrEmpty(functionFieldOrProperty))
            throw new ArgumentNullException("functionFieldOrProperty", "functionFieldOrProperty cannot be NULL or empty!");

        targetName = functionFieldOrProperty;
        order = 0;
    }

    public string GetName()
    {
        return targetName;
    }

#if UNITY_EDITOR
    public bool CheckShowIf(SerializedProperty property)
    {
        if (property == null)
            throw new NullReferenceException("Property is NULL!");

        if (targetName[0] != '#')
        {
            object obj = property.GetObjectOwningMethod(targetName);

            if (obj == null)
                return false;

            MethodInfo method = obj.GetType().GetMethod(targetName, ShowIfHelper.editorFlags);

            if (method == null)
                throw new NullReferenceException("Function " + targetName + " can't be found!");

            return (bool)method.Invoke(obj, null);
        }

        string actualTarget = targetName.Remove(0, 1);

        SerializedProperty parentProperty = property.GetArrayParentProperty();

        FieldInfo targetFieldInfo;
        PropertyInfo targetPropertyInfo;
        Type parentType;
        object result;

        if (parentProperty != null)
            parentType = parentProperty.GetBasePropertyField().GetType();

        else
            parentType = property.serializedObject.targetObject.GetType();

        targetFieldInfo = parentType.GetField(actualTarget, ShowIfHelper.editorFlags);
        targetPropertyInfo = parentType.GetProperty(actualTarget, ShowIfHelper.editorFlags);

        if (targetFieldInfo == null && targetPropertyInfo == null)
            throw new MissingReferenceException(actualTarget + " cannot be found!");

        if (targetPropertyInfo != null)
        {
            result = parentProperty != null ? targetPropertyInfo.GetValue(parentProperty.GetBasePropertyValue<object>(), null) :
                                              targetPropertyInfo.GetValue(property.serializedObject.targetObject, null);

            if (result.GetType() != typeof(bool))
                throw new OperationCanceledException(actualTarget + " is not of type bool!");

            return (bool)result;
        }

        result = parentProperty != null ? targetFieldInfo.GetValue(parentProperty.GetBasePropertyValue<object>()) :
                                            targetFieldInfo.GetValue(property.serializedObject.targetObject);

        if (result.GetType() != typeof(bool))
            throw new OperationCanceledException(actualTarget + " is not of type bool!");

        return (bool)result;
    }
#endif
}

public static class ShowIfHelper
{
#if UNITY_EDITOR
    /// <summary>
    /// Get the object that contains the supplied method name.
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="methodName">Name of the required method.</param>
    /// <returns></returns>
    public static object GetObjectOwningMethod(this SerializedProperty prop, string methodName)
    {
        string[] path = prop.UnitySerializerToReflection().Split('.');

        object obj = prop.serializedObject.targetObject as object;

        object classObject = obj.GetType().GetMethod(methodName, editorFlags) == null ? null : obj;

        FieldInfo info = null;

        for (int i = 0; i < path.Length; i++)
        {
            if (Regex.Match(path[i], @"\[\d+\]").Success)
            {
                int idx = int.Parse(Regex.Match(path[i], @"(?<=\[)\d+(?=\])").Value);

                path[i] = Regex.Replace(path[i], @"\[\d+\]", "");

                info = obj.GetType().GetField(path[i], editorFlags);
                obj = info.GetValue(obj);

                Array objs = obj as Array;

                if (objs == null)
                    return null; ;

                if (idx >= objs.Length || objs.GetValue(idx) == null)
                    return classObject;

                obj = objs.GetValue(idx);
            }

            else
            {
                info = obj.GetType().GetField(path[i], editorFlags);

                if (info == null)
                    return classObject;

                obj = info.GetValue(obj);

                if (obj == null)
                    return classObject;
            }

            if (obj.GetType().GetMethod(methodName, editorFlags) != null)
                classObject = obj;
        }

        return classObject;
    }

    /// <summary>
    /// Converts a SerializedProperty's property path to the C# Reflection equivalent.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static string UnitySerializerToReflection(this SerializedProperty prop)
    {
        return prop.propertyPath.Replace(".Array.data", "");
    }

    /// <summary>
    /// Get the SerializedProperty's FieldInfo.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static FieldInfo GetBasePropertyField(this SerializedProperty prop)
    {
        string[] separatedPaths = prop.propertyPath.Replace(".Array.data", "").Split('.');

        object reflectionTarget = prop.serializedObject.targetObject as object;
        FieldInfo fieldInfo = null;

        for (int i = 0; i < separatedPaths.Length; i++)
        {
            int idx = -1;

            if (Regex.Match(separatedPaths[i], @"\[\d+\]").Success)
            {
                idx = Convert.ToInt32(new string(separatedPaths[i].Where(char.IsDigit).ToArray()));

                separatedPaths[i] = Regex.Replace(separatedPaths[i], @"\[\d+\]", "");

                fieldInfo = reflectionTarget.GetType().GetField(separatedPaths[i], editorFlags);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);

                if (reflectionTarget == null || !reflectionTarget.GetType().GetElementType().IsValueType && (((object[])reflectionTarget).Length == 0 ||
                                                                                 ((object[])reflectionTarget)[idx] == null || idx >= ((object[])reflectionTarget).Length))
                    return null;

                Array newArray = (Array)reflectionTarget;
                reflectionTarget = newArray.GetValue(idx);
            }

            else
            {
                if (reflectionTarget == null)
                    return null;

                fieldInfo = reflectionTarget.GetType().GetField(separatedPaths[i], editorFlags);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
        }

        return fieldInfo;
    }

    /// <summary>
    /// Returns the SerializedProperty's parent. Returns null if the property is a root.
    /// </summary>
    /// <param name="prop">SerializedProperty to get parent of.</param>
    /// <returns></returns>
    public static SerializedProperty GetArrayParentProperty(this SerializedProperty prop)
    {
        if (prop.propertyPath.Split('.').Length == 1)
            return null;

        string fieldToFind = prop.IsArrayElement() ? Regex.Replace(prop.propertyPath, @"((.Array.data\[)\d+(\]))$", "") : Regex.Replace(prop.propertyPath, @"\.[^.]+$", "");

        return prop.serializedObject.FindProperty(fieldToFind);
    }

    /// <summary>
    /// Is the SerializedProperty in an Array?
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool IsArrayElement(this SerializedProperty property)
    {
        return Regex.Match(property.propertyPath, @"\[\d+\]$").Success;
    }

    /// <summary>
    /// Get the object value relative to the SerializedProperty.
    /// </summary>
    /// <typeparam name="T">Return type.</typeparam>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static T GetBasePropertyValue<T>(this SerializedProperty prop)
    {
        string[] separatedPaths = prop.propertyPath.Replace(".Array.data", "").Split('.');

        object reflectionTarget = prop.serializedObject.targetObject as object;

        for (int i = 0; i < separatedPaths.Length; i++)
        {
            int idx = -1;
            FieldInfo fieldInfo = null;

            if (Regex.Match(separatedPaths[i], @"\[\d+\]").Success)
            {
                idx = Convert.ToInt32(new string(separatedPaths[i].Where(x => char.IsDigit(x)).ToArray()));

                separatedPaths[i] = Regex.Replace(separatedPaths[i], @"\[\d+\]", "");

                fieldInfo = reflectionTarget.GetType().GetField(separatedPaths[i], editorFlags);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);

                if (reflectionTarget == null || idx >= ((object[])reflectionTarget).Length || ((object[])reflectionTarget)[idx] == null)
                    return default(T);

                reflectionTarget = ((object[])reflectionTarget)[idx];
            }

            else
            {
                if (reflectionTarget == null)
                    return default(T);

                fieldInfo = reflectionTarget.GetType().GetField(separatedPaths[i], editorFlags);

                if (fieldInfo == null)
                    return default(T);

                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
        }

        return (T)reflectionTarget;
    }
#endif

    public static BindingFlags editorFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
}