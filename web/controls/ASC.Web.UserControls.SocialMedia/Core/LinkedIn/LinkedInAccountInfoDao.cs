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
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.SocialMedia.LinkedIn
{
    class LinkedInAccountInfoDao : BaseDao
    {
        public LinkedInAccountInfoDao(int tenantID, String storageKey) : base(tenantID, storageKey) { }

        public void CreateNewAccountInfo(LinkedInAccountInfo accountInfo)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DeleteAccountInfo(accountInfo.AssociatedID);

                SqlInsert cmdInsert = Insert("sm_linkedinaccounts")
                           .ReplaceExists(true)
                           .InColumnValue("access_token", accountInfo.AccessToken)
                           .InColumnValue("access_token_secret", accountInfo.AccessTokenSecret)
                           .InColumnValue("user_id", accountInfo.UserID)
                           .InColumnValue("associated_id", accountInfo.AssociatedID)
                           .InColumnValue("user_name", accountInfo.UserName)
                           .InColumnValue("token_type", (int)accountInfo.TokenType);

                DbManager.ExecuteNonQuery(cmdInsert);

                tx.Commit();
            }
        }

        public string GetTokenSecret(string accessToken)
        {
            SqlQuery sqlQuery = Query("sm_linkedinaccounts")
               .Select("access_token_secret")
               .Where(Exp.Eq("access_token", accessToken));

            var tokenSecrets = DbManager.ExecuteList(sqlQuery);
            if (tokenSecrets.Count == 0)
                return null;

            return Convert.ToString(tokenSecrets[0][0]);
        }

        public LinkedInAccountInfo.TokenTypes GetTokenType(string token)
        {
            SqlQuery sqlQuery = Query("sm_linkedinaccounts")
                .Select("token_type")
                 .Where("access_token", token);
            var tokenTypes = DbManager.ExecuteList(sqlQuery);

            if (tokenTypes.Count == 0)
                return LinkedInAccountInfo.TokenTypes.InvalidToken;

            return (LinkedInAccountInfo.TokenTypes)Convert.ToInt32(tokenTypes[0][0]);
        }

        public void DeleteAccountInfo(Guid associatedID)
        {
            DbManager.ExecuteNonQuery(Delete("sm_linkedinaccounts").Where(Exp.Eq("associated_id", associatedID)));
        }

        public LinkedInAccountInfo GetAccountInfo(Guid associatedID)
        {
            var accounts = DbManager.ExecuteList(GetAccountQuery(associatedID));
            return accounts.Count > 0 ? ToLinkedInAccountInfo(accounts[0]) : null;
        }

        public void UpdateAccountInfo(LinkedInAccountInfo accountInfo)
        {
            SqlUpdate cmdUpdate = Update("sm_linkedinaccounts")
                           .Set("user_id", accountInfo.UserID)
                           .Set("user_name", accountInfo.UserName)
                           .Where("access_token", accountInfo.AccessToken)
                           .Where("access_token_secret", accountInfo.AccessTokenSecret);

            DbManager.ExecuteNonQuery(cmdUpdate);
        }


        private SqlQuery GetAccountQuery(Guid associatedID)
        {
            SqlQuery sqlQuery = Query("sm_linkedinaccounts")
                .Select(
                    "access_token",
                    "access_token_secret",
                    "user_id",
                    "associated_id",
                    "user_name",
                    "token_type"
                )
                .Where(Exp.Eq("associated_id", associatedID))
                .Where(Exp.Eq("token_type", (int)LinkedInAccountInfo.TokenTypes.AccessToken));

            return sqlQuery;
        }

        private static LinkedInAccountInfo ToLinkedInAccountInfo(object[] row)
        {
            LinkedInAccountInfo accountInfo = new LinkedInAccountInfo
            {
                AccessToken = Convert.ToString(row[0]),
                AccessTokenSecret = Convert.ToString(row[1]),
                UserID = Convert.ToString(row[2]),
                AssociatedID = ToGuid(row[3]),
                UserName = Convert.ToString(row[4]),
                TokenType = (LinkedInAccountInfo.TokenTypes)Convert.ToInt32(row[5])
            };

            return accountInfo;
        }
    }
}
