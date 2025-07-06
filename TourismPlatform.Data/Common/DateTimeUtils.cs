using System;
using System.Reflection;

namespace TourismPlatform.Data.Common;

public static class DateTimeUtils
{
    public static void EnsureUtcDateTimes(object obj)
    {
        if (obj == null) return;

        var properties = obj.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanRead && p.CanWrite && 
                                       (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)));

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);

            if (value is DateTime dt)
            {
                if (dt.Kind == DateTimeKind.Unspecified)
                {
                    // Mantiene el mismo valor, solo marca el Kind como Utc
                    prop.SetValue(obj, DateTime.SpecifyKind(dt, DateTimeKind.Utc));
                }
                else if (dt.Kind == DateTimeKind.Local)
                {
                    prop.SetValue(obj, dt.ToUniversalTime());
                }
            }
        }
    }
}

