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
using System.Linq;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.CRM.Core.Dao;
using ASC.Core.Tenants;
using ASC.Specific;
using System.Runtime.Serialization;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Api.CRM
{

    public class ReportFilter
    {

        public String Name { get; set; }
        public String Value { get; set; }

    }




    public partial class CRMApi
    {

        //[Read("report/workload")]
        //public ReportWrapper BuildWorkLoadReport(Guid responsibleID)
        //{
        //    // CRMReportResource


        //    throw new NotImplementedException();
        //}

        //[Read("report/usertasks")]
        //public ReportWrapper BuildUserTasksReport(
        //    Guid responsibleID,
        //    DateTime fromDate,
        //    DateTime toDate,
        //    bool? isClosed,
        //    bool showWithoutResponsible)
        //{


        //    var reportData = DaoFactory.GetReportDao().BuildTasksReport(
        //        responsibleID,
        //        fromDate,
        //        toDate,
        //        isClosed,
        //        showWithoutResponsible);

        //    var report = new ReportWrapper
        //                     {
        //                         ReportTitle = CRMReportResource.Report_UserTasks_Title,
        //                         ReportDescription = CRMReportResource.Report_ContactPopulate_Description,
        //                         Data = reportData.ConvertAll(row => new
        //                         {
        //                             Title = row[0],
        //                             Description = row[1]

        //                             //Date = (ApiDateTime)TenantUtil.DateTimeFromUtc(DateTime.Parse(Convert.ToString(row[0]))),
        //                             // Count = row[1]
        //                         }),
        //                         Lables = new List<String>
        //                                     {
        //                                          CRMCommonResource.Date,
        //                                          CRMCommonResource.Count                                                
        //                                     }
        //                     };

        //    return report;


        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="isCompany"></param>
        /// <param name="tagName"></param>
        /// <param name="contactType"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Read("report/contactpopulate")]
        public ReportWrapper BuildContactPopulateReport(
                                                        ApiDateTime fromDate,
                                                        ApiDateTime toDate,
                                                        bool? isCompany,
                                                        String tagName,
                                                        int contactType)
        {

            var reportData = DaoFactory.GetReportDao().BuildContactPopulateReport(fromDate, toDate, isCompany, tagName,
                                                                                  contactType);

            var report = new ReportWrapper
                             {
                                 ReportTitle = CRMReportResource.Report_ContactPopulate_Title,
                                 ReportDescription = CRMReportResource.Report_ContactPopulate_Description,
                                 Data = reportData.ConvertAll(row => new
                                                                     {
                                                                         Date = (ApiDateTime)TenantUtil.DateTimeFromUtc(DateTime.Parse(Convert.ToString(row[0]))),
                                                                         Count = row[1]
                                                                     }),
                                 Lables = new List<string>
                                             {
                                                  CRMCommonResource.Date,
                                                  CRMCommonResource.Count                                                
                                             }

                             };

            return report;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responsibleID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Read("report/salesforecastbymonth")]
        public ReportWrapper BuildSalesForecastByMonthReport(Guid responsibleID, DateTime fromDate, DateTime toDate)
        {

            var reportData = DaoFactory.GetReportDao().
                BuildSalesForecastByMonthReport(
                                                responsibleID,
                                                fromDate,
                                                toDate
                                               );

            var report = new ReportWrapper
            {
                ReportTitle = CRMReportResource.Report_SalesForecastByMonth_Title,
                ReportDescription = CRMReportResource.Report_SalesForecastByMonth_Description,
                Data = reportData.ConvertAll(row => new
                {
                    month =row[0],
                    amount = row[1],
                    count = row[2],
                    percent = row[3]
                }),
                Lables = new List<string>
                                         {
                                              CRMCommonResource.Date,
                                              CRMCommonResource.Count                                                
                                         }

            };

            return report;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Read("report/salesforecastbyclient")]
        public ReportWrapper BuildSalesForecastByClientReport(DateTime fromDate, DateTime toDate)
        {

            var reportData = DaoFactory.GetReportDao().
                BuildSalesForecastByClientReport(
                                                
                                                fromDate,
                                                toDate
                                               );

            var report = new ReportWrapper
            {
                ReportTitle = CRMReportResource.Report_SalesForecastByMonth_Title,
                ReportDescription = CRMReportResource.Report_SalesForecastByMonth_Description,
                Data = reportData.ConvertAll(row => new
                {
                    client = row[0],
                    amount = row[1],
                    count = row[2],
                    percent = row[3]
                }),
                Lables = new List<string>
                                         {
                                              CRMCommonResource.Date,
                                              CRMCommonResource.Count                                                
                                         }

            };

            return report;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Read("report/salesforecastbymanager")]
        public ReportWrapper BuildSalesForecastByManagerReport(DateTime fromDate, DateTime toDate)
        {

            var reportData = DaoFactory.GetReportDao().
                BuildSalesForecastByManagerReport(
                                                
                                                fromDate,
                                                toDate
                                               );

            var report = new ReportWrapper
            {
                ReportTitle = CRMReportResource.Report_SalesForecastByMonth_Title,
                ReportDescription = CRMReportResource.Report_SalesForecastByMonth_Description,
                Data = reportData.ConvertAll(row => new
                {
                    manager = row[0],
                    amount = row[1],
                    count = row[2],
                    percent = row[3]
                }),
                Lables = new List<string>
                                         {
                                              CRMCommonResource.Date,
                                              CRMCommonResource.Count                                                
                                         }

            };

            return report;

        }


    }
}
