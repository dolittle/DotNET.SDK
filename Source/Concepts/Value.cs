﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dolittle.SDK.Concepts
{
    /// <summary>
    /// A base class for providing value object equality semantics.  A Value Object does not have an identity and its value comes from its properties.
    /// </summary>
    /// <typeparam name="T">The specific type to provide value object equality semantics to.</typeparam>
    public abstract class Value<T> : IEquatable<T>
        where T : Value<T>
    {
        static IList<FieldInfo> _fields;

        static Value() => _fields = new List<FieldInfo>();

        /// <summary>
        /// Equates two objects to check that they are equal.
        /// </summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>True if the objects are equal, false is they are not.</returns>
        public static bool operator ==(Value<T> left, Value<T> right) => ReferenceEquals(left, right) || left.Equals(right);

        /// <summary>
        /// Equates two objects to check that they are not equal.
        /// </summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>True if the objects are not equal, false is they are.</returns>
        public static bool operator !=(Value<T> left, Value<T> right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is T typed)
            {
                return Equals(typed);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var fields = GetFields().Select(field => field.GetValue(this)).Where(value => value != null).ToList();
            fields.Add(GetType());
            return HashCodeHelper.Generate(fields.ToArray());
        }

        /// <inheritdoc/>
        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;

            var t = GetType();
            var otherType = other.GetType();

            if (t != otherType)
                return false;

            var fields = GetFields();

            foreach (var field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{[Type: " + GetType() + "]");
            foreach (var field in GetFields())
            {
                sb.Append($"{{ {RemoveBackingAutoBackingFieldPropertyName(field.Name)} : {field.GetValue(this) ?? "[NULL]"} }}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        IEnumerable<FieldInfo> GetFields()
        {
            if (_fields.Count == 0)
                _fields = new List<FieldInfo>(BuildFieldCollection());

            return _fields;
        }

        IEnumerable<FieldInfo> BuildFieldCollection()
        {
            var t = typeof(T);
            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                var typeInfo = t.GetTypeInfo();

                fields.AddRange(typeInfo.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                var fieldInfoCache = typeInfo.GetField("_fields");
                fields.Remove(fieldInfoCache);
                t = typeInfo.BaseType;
            }

            return fields;
        }

        string RemoveBackingAutoBackingFieldPropertyName(string fieldName)
        {
            var field = fieldName.TrimStart('<');
            return field.Replace(">k__BackingField", string.Empty, StringComparison.InvariantCulture);
        }
    }
}