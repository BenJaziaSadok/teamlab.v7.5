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
using System.Runtime.Serialization;
using ASC.Web.CRM.Classes;

namespace ASC.Api.CRM
{

    /// <summary>
    ///  Currency information
    /// </summary>
    [DataContract(Name = "currencyInfo", Namespace = "")]
    public class CurrencyInfoWrapper
    {

        public CurrencyInfoWrapper()
        {

        }

        public CurrencyInfoWrapper(CurrencyInfo currencyInfo)
        {
            Abbreviation = currencyInfo.Abbreviation;
            CultureName = currencyInfo.CultureName;
            Symbol = currencyInfo.Symbol;
            Title = currencyInfo.Title;
            IsConvertable = currencyInfo.IsConvertable;
        }

        [DataMember]
        public String Title { get; set; }

        [DataMember]
        public String Symbol { get; set; }

        [DataMember]
        public String Abbreviation { get; set; }

        [DataMember]
        public String CultureName { get; set; }

        [DataMember]
        public bool IsConvertable { get; set; } 

        public static CurrencyInfoWrapper GetSample()
        {
            return new CurrencyInfoWrapper
                       {
                           Title = "Chinese Yuan",
                           Abbreviation = "CNY",
                           Symbol = "¥",
                           CultureName = "CN",
                           IsConvertable = true
                       };
        }

    }


    /// <summary>
    ///  Currency rate information
    /// </summary>
    [DataContract(Name = "currencyRateInfo", Namespace = "")]
    public class CurrencyRateInfoWrapper: CurrencyInfoWrapper
    {

        public CurrencyRateInfoWrapper()
        { }

        public CurrencyRateInfoWrapper(CurrencyInfo currencyInfo, Decimal rate): base(currencyInfo)
        {
            Rate = rate;
        }

        [DataMember]
        public Decimal Rate { get; set; }
    }
}
