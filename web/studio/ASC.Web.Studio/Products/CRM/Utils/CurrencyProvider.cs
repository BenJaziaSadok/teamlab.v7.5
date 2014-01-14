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

#region Import

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using System.Xml;
using ASC.Web.CRM.Resources;
using log4net;
using System.Security.Principal;

#endregion

namespace ASC.Web.CRM.Classes
{
    [Serializable]
    public class CurrencyInfo
    {
        private String _resourceKey;

        public String Title
        {
            get
            {

                if (String.IsNullOrEmpty(_resourceKey))
                    return String.Empty;

                return CRMCommonResource.ResourceManager.GetString(_resourceKey);

            }
        }

        public string Symbol { get; set; }
        public string Abbreviation { get; set; }
        public string CultureName { get; set; }
        public bool IsConvertable { get; set; }

        public CurrencyInfo()
        {

            IsConvertable = true;

        }

        public CurrencyInfo(string resourceKey, string abbreviation, string symbol, string cultureName)
        {
            _resourceKey = resourceKey;
            Symbol = symbol;
            Abbreviation = abbreviation;
            CultureName = cultureName;
            IsConvertable = true;
        }


        public override bool Equals(object obj)
        {
            var ci = obj as CurrencyInfo;
            return ci != null && string.Compare(Title, ci.Title, true) == 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(Abbreviation, "-", Title);
        }
    }


    public static class CurrencyProvider
    {

        #region Members

        private static readonly ILog _log = LogManager.GetLogger(typeof(CurrencyProvider));
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<String, CurrencyInfo> _currencies;
        private static Dictionary<String, Decimal> _exchangeRates;
        private static DateTime _publisherDate;

        #endregion

        #region Constructor

        static CurrencyProvider()
        {
            _currencies = new[]
            {
                new CurrencyInfo("Currency_UnitedArabEmiratesDirham", "AED", "د.إ", "AE"),
                new CurrencyInfo("Currency_ArmenianDram", "AMD", "dram", "AM"){IsConvertable=false},
                new CurrencyInfo("Currency_ArgentinianPeso", "ARS", "$a","AR"),
                new CurrencyInfo("Currency_AustralianDollar","AUD","A$","AU"),
                new CurrencyInfo("Currency_AzerbaijaniManat","AZN","m","AZ"){IsConvertable=false},
                new CurrencyInfo("Currency_BangladeshiTaka","BDT","Tk","BD"){IsConvertable=false},
                new CurrencyInfo("Currency_BrazilianReal","BRL","R$","BR"),
                new CurrencyInfo("Currency_BelarusianRuble","BYR","Br","BY"){IsConvertable=false},
                new CurrencyInfo("Currency_CanadianDollar","CAD","C$","CA"),  
                new CurrencyInfo("Currency_SwissFranc","CHF","Fr","CH"),
                new CurrencyInfo("Currency_ChileanPeso","CLP","$","CL"),
                new CurrencyInfo("Currency_ChineseYuan","CNY","¥","CN"),
                new CurrencyInfo("Currency_CzechKoruna","CZK","Kc","CZ"),
                new CurrencyInfo("Currency_DanishKrone","DKK","kr","DK"),
                new CurrencyInfo("Currency_AlgerianDinar","DZD","د.ج","DZ"){IsConvertable=false},
                new CurrencyInfo("Currency_Euro","EUR","€","EU"),
                new CurrencyInfo("Currency_PoundSterling","GBP","£","GB"),
                new CurrencyInfo("Currency_GeorgianLari","GEL","ლარი","GE"){IsConvertable=false},
                new CurrencyInfo("Currency_HongKongDollar","HKD","HK$","HK"),
                new CurrencyInfo("Currency_HungarianForint","HUF","Ft","HU"),
                new CurrencyInfo("Currency_IndonesianRupiah","IDR","Rp","ID"),
                new CurrencyInfo("Currency_IsraeliSheqel","ILS","₪","IL"),
                new CurrencyInfo("Currency_IndianRupee","INR","₨","IN"),
                new CurrencyInfo("Currency_JapaneseYen","JPY","¥","JP"),
                new CurrencyInfo("Currency_SouthKoreanWon","KRW","₩","KR"),
                new CurrencyInfo("Currency_KuwaitiDinar","KWD","K.D.","KW"),
                new CurrencyInfo("Currency_KazakhstaniTenge","KZT","тңг","KZ"){IsConvertable=false},
                new CurrencyInfo("Currency_LithuanianLitas","LTL","Lt","LT"),
                new CurrencyInfo("Currency_LatvianLats","LVL","Ls","LV"),
                new CurrencyInfo("Currency_MoroccanDirham","MAD","د.م","MA"),
                new CurrencyInfo("Currency_MalagasyAriary","MGA","Ar","MG"){IsConvertable=false},
                new CurrencyInfo("Currency_MalaysianRinggit","MYR","RM","MY"),
                new CurrencyInfo("Currency_MexicanPeso","MXN","MEX$","MX"),
                new CurrencyInfo("Currency_MauritianRupee","MUR","Rs","MU"),
                new CurrencyInfo("Currency_NorwegianKrone","NOK","kr","NO"),
                new CurrencyInfo("Currency_NewZealandDollar","NZD","NZ$","NZ"),
                new CurrencyInfo("Currency_PakistaniRupee","PKR","Rp.","PK"),
                new CurrencyInfo("Currency_PhilippinePeso","PHP","P","PH"),
                new CurrencyInfo("Currency_PolishZloty","PLN","zł","PL"),
                new CurrencyInfo("Currency_Rouble","RUB","руб.","RU"),
                new CurrencyInfo("Currency_SaudiRiyal","SAR","ر.س","SA"),
                new CurrencyInfo("Currency_SwedishKrona","SEK","kr","SE"),
                new CurrencyInfo("Currency_SingaporeDollar","SGD","S$","SG"),
                new CurrencyInfo("Currency_ThaiBaht","THB","฿","TH"),
                new CurrencyInfo("Currency_TurkishNewLira","TRY","YTL","TR"),
                new CurrencyInfo("Currency_VenezuelanBolivar","VEF","Bs.","VE"),
                new CurrencyInfo("Currency_VietnameseDong","VND","₫","VN"),
                new CurrencyInfo("Currency_UkraineHryvnia","UAH","грн.","UA"),
                new CurrencyInfo("Currency_UnitedStatesDollar","USD","$","US"),
                new CurrencyInfo("Currency_UzbekistaniSom","UZS","som","UZ"){IsConvertable=false},
                new CurrencyInfo("Currency_SouthAfricanRand","ZAR","R","ZA")
            }
            .ToDictionary(c => c.Abbreviation);
        }

        #endregion

        #region Property

        public static DateTime GetPublisherDate
        {
            get { return _publisherDate; }
        }

        #endregion

        #region Public Methods

        public static CurrencyInfo Get(string currencyAbbreviation)
        {
            if (!_currencies.ContainsKey(currencyAbbreviation))
                return null;

            return _currencies[currencyAbbreviation];
        }

        public static List<CurrencyInfo> GetAll()
        {
            return _currencies.Values.OrderBy(v => v.Abbreviation).ToList();
        }

        public static Dictionary<CurrencyInfo, Decimal> MoneyConvert(CurrencyInfo baseCurrency)
        {
            if (baseCurrency == null) throw new ArgumentNullException("baseCurrency");
            if (!_currencies.ContainsKey(baseCurrency.Abbreviation)) throw new ArgumentOutOfRangeException("baseCurrency", "Not found.");

            var result = new Dictionary<CurrencyInfo, Decimal>();
            var rates = GetExchangeRates();
            foreach (var ci in GetAll())
            {

                if (baseCurrency.Title == ci.Title)
                {
                    result.Add(ci, 1);

                    continue;
                }

                var key = String.Format("{1}/{0}", baseCurrency.Abbreviation, ci.Abbreviation);

                if (!rates.ContainsKey(key))
                    continue;

                result.Add(ci, Math.Round(rates[key], 2));
            }
            return result;
        }


        public static bool IsConvertable(String abbreviation)
        {
            var findedItem = _currencies.Keys.ToList().Find(item => String.Compare(abbreviation, item) == 0);

            if (findedItem == null)
                throw new ArgumentException(abbreviation);

            return _currencies[findedItem].IsConvertable;
        }

        public static Decimal MoneyConvert(decimal amount, string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.Compare(from, to, true) == 0) return amount;

            var rates = GetExchangeRates();

            if (from.Contains('-')) from = new RegionInfo(from).ISOCurrencySymbol;
            if (to.Contains('-')) to = new RegionInfo(to).ISOCurrencySymbol;
            var key = string.Format("{0}/{1}", to, from);

            return Math.Round(rates[key] * amount, 4, MidpointRounding.AwayFromZero);
        }

        public static Decimal MoneyConvertToDefaultCurrency(decimal amount, string from)
        {
            return MoneyConvert(amount, from, Global.TenantSettings.DefaultCurrency.Abbreviation);
        }

        #endregion

        #region Private Methods

        private static bool ObsoleteData()
        {
            return _exchangeRates == null || (DateTime.UtcNow.Date.Subtract(_publisherDate.Date).Days > 0);
        }

        private static Dictionary<String, Decimal> GetExchangeRates()
        {
            if (ObsoleteData())
            {
                lock (_syncRoot)
                {
                    if (ObsoleteData())
                    {
                        try
                        {
                            _exchangeRates = new Dictionary<string, decimal>();

                            var tmppath = Environment.GetEnvironmentVariable("TEMP");
                            if (string.IsNullOrEmpty(tmppath))
                            {
                                tmppath = Path.GetTempPath();
                            }
                            tmppath = Path.Combine(tmppath, WindowsIdentity.GetCurrent().Name + "\\Teamlab\\crm\\Exchange_Rates\\");

                            if (_publisherDate == default(DateTime))
                            {
                                try
                                {
                                    var timefile = Path.Combine(tmppath, "last.time");
                                    if (File.Exists(timefile))
                                    {
                                        _publisherDate = DateTime.ParseExact(File.ReadAllText(timefile), "o", null);
                                    }
                                }
                                catch (Exception err)
                                {
                                    LogManager.GetLogger("ASC.CRM").Error(err);
                                }
                            }

                            var regex = new Regex("= (?<Currency>([\\s\\.\\d]*))");
                            var updateEnable = WebConfigurationManager.AppSettings["crm.update.currency.info.enable"] != "false";
                            foreach (var ci in _currencies.Values.Where(c => c.IsConvertable))
                            {
                                var filepath = Path.Combine(tmppath, ci.Abbreviation + ".xml");

                                if (updateEnable && 0 < (DateTime.UtcNow.Date - _publisherDate.Date).TotalDays || !File.Exists(filepath))
                                {
                                    DownloadRSS(ci.Abbreviation, filepath);
                                }

                                if (!File.Exists(filepath))
                                {
                                    continue;
                                }

                                using (var reader = XmlReader.Create(filepath))
                                {
                                    var feed = SyndicationFeed.Load(reader);
                                    if (feed != null)
                                    {
                                        foreach (var item in feed.Items)
                                        {
                                            var currency = regex.Match(item.Summary.Text).Groups["Currency"].Value.Trim();
                                            _exchangeRates.Add(item.Title.Text, Convert.ToDecimal(currency, CultureInfo.InvariantCulture.NumberFormat));
                                        }
                                    }

                                    _publisherDate = feed.LastUpdatedTime.DateTime;
                                }
                            }

                            try
                            {
                                var timefile = Path.Combine(tmppath, "last.time");
                                File.WriteAllText(timefile, _publisherDate.ToString("o"));
                            }
                            catch (Exception err)
                            {
                                LogManager.GetLogger("ASC.CRM").Error(err);
                            }
                        }
                        catch (Exception error)
                        {
                            LogManager.GetLogger("ASC.CRM").Error(error);
                            _publisherDate = DateTime.UtcNow;
                        }
                    }
                }
            }

            return _exchangeRates;
        }

        private static void DownloadRSS(string currency, string filepath)
        {

            try
            {
                var dir = Path.GetDirectoryName(filepath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var destinationURI = new Uri(String.Format("http://themoneyconverter.com/rss-feed/{0}/rss.xml", currency));

                var request = (HttpWebRequest)WebRequest.Create(destinationURI);
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.MaximumAutomaticRedirections = 2;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:8.0) Gecko/20100101 Firefox/8.0";

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = new StreamReader(response.GetResponseStream()))
                {
                    var data = responseStream.ReadToEnd();

                    File.WriteAllText(filepath, data);

                }
            }
            catch (Exception error)
            {
                _log.Error(error);
            }
        }

        #endregion
    }
}
