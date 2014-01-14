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

using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace ASC.Api.Impl.Serializers
{
    public class JsonStringConverter : JsonConverter
    {
        private string GetEscapedJsonString(string str)
        {
            //THIS MAGIC FUNCTION IS REAPED FROM:
            //System.Runtime.Serialization.Json.XmlJsonWriter

            var writer = new StringWriter();
            var chars = str.ToCharArray();
            writer.Write('"');

            int num = 0;
            int index;
            for (index = 0; index < str.Length; ++index)
            {
                char ch = chars[index];
                if ((int)ch <= 47)
                {
                    if ((int)ch == 47 || (int)ch == 34)
                    {
                        writer.Write(chars, num, index - num);
                        writer.Write((char)92);
                        writer.Write(ch);
                        num = index + 1;
                    }
                    else if ((int)ch < 32)
                    {
                        writer.Write(chars, num, index - num);
                        writer.Write((char)92);
                        writer.Write((char)117);
                        writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:x4}", new object[1]
                                                                                               {
                                                                                                   (int) ch
                                                                                               }));
                        num = index + 1;
                    }
                }
                else if ((int)ch == 92)
                {
                    writer.Write(chars, num, index - num);
                    writer.Write((char)92);
                    writer.Write(ch);
                    num = index + 1;
                }
                else if ((int)ch >= 55296 && ((int)ch <= 57343 || (int)ch >= 65534))
                {
                    writer.Write(chars, num, index - num);
                    writer.Write((char)92);
                    writer.Write((char)117);
                    writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:x4}", new object[1]
                                                                                           {
                                                                                               (int) ch
                                                                                           }));
                    num = index + 1;
                }
            }
            if (num < index)
                writer.Write(chars, num, index - num);
            writer.Write('"');
            return writer.GetStringBuilder().ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueString = value as string;
            if (valueString != null)
            {
                writer.WriteRawValue(GetEscapedJsonString((string)value));//Write raw here, so no aditional encoding done by Json.net
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Convert.ToString(existingValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}