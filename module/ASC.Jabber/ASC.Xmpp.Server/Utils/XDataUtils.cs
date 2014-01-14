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

using ASC.Xmpp.Core.protocol.x.data;

namespace ASC.Xmpp.Server.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

    public class XDataFromsExample
	{
		[XDataUtils.XDataAnyOf("any", "all", "one of")]
		public string[] many { get; set; }

		[XDataUtils.XDataOneOf("any", "all", "one of")]
		public string oneof { get; set; }

		[XDataUtils.XDataFixed]
		public string data1 { get; set; }

		[XDataUtils.XDataMultiline]
		public string data2 { get; set; }

		[XDataUtils.XDataPassword]
		public string data3 { get; set; }

		[XDataUtils.XDataDescription("Label test")]
		public string data4 { get; set; }

		public XDataFromsExample()
		{
			many = new string[] { "any" };
			oneof = "all";
			data1 = "fixed!";
		}

	}



	public class XDataUtils
	{
		public class XDataDescriptionAttribute : Attribute
		{
			public string Description { get; set; }

			public XDataDescriptionAttribute(string description)
			{
				Description = description;
			}
		}

		public class XDataOneOfAttribute : Attribute
		{
			public string[] Variants { get; set; }

			public XDataOneOfAttribute(params string[] variants)
			{
				Variants = variants;
			}
		}

		public class XDataAnyOfAttribute : Attribute
		{
			public string[] Variants { get; set; }

			public XDataAnyOfAttribute(params string[] variants)
			{
				Variants = variants;
			}
		}

		public class XDataMultiline : Attribute
		{
		}

		public class XDataFixed : Attribute
		{
		}

		public class XDataPassword : Attribute
		{
		}

		public static void FillDataTo(object dataForm, string prefix, Data data)
		{
			if (data.Type == XDataFormType.submit)
			{
				//Gen prop map
				PropertyInfo[] props =
					dataForm.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty |
													 BindingFlags.Public);
				Dictionary<string, PropertyInfo> propsVar = new Dictionary<string, PropertyInfo>();
				foreach (PropertyInfo prop in props)
				{
					if (prop.CanWrite)
					{
						propsVar.Add(string.Format("{0}#{1}", prefix, prop.Name), prop);
					}
				}

				Field[] fields = data.GetFields();
				foreach (var field in fields)
				{
					if (propsVar.ContainsKey(field.Var))
					{
						PropertyInfo prop = propsVar[field.Var];
						if (prop.PropertyType == typeof(bool))
						{
							string val = field.GetValue();
							if (!string.IsNullOrEmpty(val))
							{
								prop.SetValue(dataForm, val == "1", null);
							}
						}
						else if (prop.PropertyType == typeof(string))
						{
							string val = field.GetValue();
							if (!string.IsNullOrEmpty(val))
							{
								prop.SetValue(dataForm, val, null);
							}
						}
						else if (prop.PropertyType == typeof(string[]))
						{
							string[] val = field.GetValues();
							if (val != null)
							{
								prop.SetValue(dataForm, val, null);
							}
						}

					}
				}
			}
		}

		public static Data GetDataForm(object dataForm, string prefix)
		{
			Data data = new Data(XDataFormType.form);

			//Go through public vars
			PropertyInfo[] props = dataForm.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty |
											 BindingFlags.Public);
			foreach (PropertyInfo prop in props)
			{
				if (prop.CanRead)
				{
					Field field = new Field(FieldType.Unknown);

					field.Var = string.Format("{0}#{1}", prefix, prop.Name);
					object propValue = prop.GetValue(dataForm, null);

					foreach (var attribute in prop.GetCustomAttributes(false))
					{
						if (attribute is XDataDescriptionAttribute)
						{
							field.Label = (attribute as XDataDescriptionAttribute).Description;
						}
						else if (attribute is XDataOneOfAttribute)
						{
							field.Type = FieldType.List_Single;
							field.FieldValue = (string)propValue;
							foreach (var vars in (attribute as XDataOneOfAttribute).Variants)
							{
								field.AddOption(vars, vars);
							}
						}
						else if (attribute is XDataAnyOfAttribute)
						{
							field.Type = FieldType.List_Multi;
							field.AddValues((string[])propValue);
							foreach (var vars in (attribute as XDataAnyOfAttribute).Variants)
							{
								field.AddOption(vars, vars);
							}
						}
						else if (attribute is XDataMultiline)
						{
							field.Type = FieldType.Text_Multi;
							field.FieldValue = (string)propValue;
						}
						else if (attribute is XDataPassword)
						{
							field.Type = FieldType.Text_Private;
							field.FieldValue = (string)propValue;
						}
						else if (attribute is XDataFixed)
						{
							field.Type = FieldType.Fixed;
							field.FieldValue = (string)propValue;
						}
					}
					if (field.Type == FieldType.Unknown)
					{
						if (prop.PropertyType == typeof(bool))
						{
							field.Type = FieldType.Boolean;
							field.FieldValue = (bool)propValue ? "1" : "0";
						}
						else if (prop.PropertyType == typeof(string))
						{
							field.Type = FieldType.Text_Single;
							field.FieldValue = (string)propValue;
						}
					}
					if (field.Label == null)
					{
						field.Label = prop.Name;
					}
					data.AddField(field);
				}
			}

			return data;
		}
	}
}