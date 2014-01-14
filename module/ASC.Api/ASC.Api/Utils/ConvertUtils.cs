/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

#region usings

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Web.Script.Serialization;
using ASC.Api.Exceptions;

#endregion

namespace ASC.Api.Utils
{
    public static class ConvertUtils
    {

        public static object GetConverted(string value, ParameterInfo parameterInfo)
        {
            try
            {
                return GetConverted(value, parameterInfo.ParameterType);
            }
            catch (ApiArgumentMismatchException e)
            {
                throw new ApiArgumentMismatchException(parameterInfo.Name,
                                                             parameterInfo.ParameterType, e);
            }
        }

        public static object GetConverted(string value, PropertyInfo propertyInfo)
        {
            try
            {
                return GetConverted(value, propertyInfo.PropertyType);
            }
            catch (ApiArgumentMismatchException e)
            {
                throw new ApiArgumentMismatchException(propertyInfo.Name,
                                                             propertyInfo.PropertyType, e);
            }
        }

        public static object GetConverted(string value, Type type)
        {
            try
            {
                return TypeDescriptor.GetConverter(type).ConvertFromString(new DummyTypeDescriptorContext(), CultureInfo.InvariantCulture, value);
            }
            catch (Exception e)
            {
                throw new ApiArgumentMismatchException(value,type,e);
            }
        }

        internal class DummyTypeDescriptorContext : ITypeDescriptorContext
        {
            private readonly IServiceProvider _serviceProvider = null;
            private readonly object _component = null;
            private readonly PropertyDescriptor _propDescriptor = null;

            public IContainer Container
            {
                get
                {
                    return (IContainer)null;
                }
            }

            public object Instance
            {
                get
                {
                    return this._component;
                }
            }

            public PropertyDescriptor PropertyDescriptor
            {
                get
                {
                    return this._propDescriptor;
                }
            }


            public void OnComponentChanged()
            {
            }

            public bool OnComponentChanging()
            {
                return true;
            }

            public object GetService(Type serviceType)
            {
                if (this._serviceProvider != null)
                    return this._serviceProvider.GetService(serviceType);
                else
                    return (object)null;
            }
        }
    }
}