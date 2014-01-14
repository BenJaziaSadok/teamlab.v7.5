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

namespace ASC.Api.Utils
{
    public static class Update
    {
        public static T IfNotEquals<T>(T current, T @new)
        {
            if (!Equals(current,@new))
            {
                current = @new;
                return @new;
            }
            return current;
        }

        public static T IfNotEmptyAndNotEquals<T>(T current, T @new)
        {
            if (Equals(@new,default(T))) return current;

            if (!Equals(current, @new))
            {
                current = @new;
                return @new;
            }
            return current;
        }
    }
}