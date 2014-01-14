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

#if DEBUG
using ASC.SocialMedia.Twitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace ASC.SocialMedia.Tests
{
    [TestClass]
    public class TwitterTest
    {
        private const int MaxIterations = 1;
        private const int MaxErrorsCount = 3;


        [TestMethod]
        public void MultiThreadTest()
        {
            var ai = new TwitterApiInfo
            {
                ConsumerKey = "hYd5c8gYxhy3T2Nvj8wLA",
                ConsumerSecret = "KZGPvAN6kgioWlf0zrGYOzKevLatexuuuJlIhJQg"
            };

            var provider = new TwitterDataProvider(ai);

            var messages = provider.GetUserTweets(40648902, null, 10);
            //List<Message> messages = provider.GetUserHomeTimeLine(10);

            Action dlgFirstThread = StartFirstThread;
            Action dlgSecondThread = StartSecondThread;

            var arFirstThread = dlgFirstThread.BeginInvoke(null, null);
            var arSecondThread = dlgSecondThread.BeginInvoke(null, null);

            var waitHandles = new WaitHandle[] { arFirstThread.AsyncWaitHandle, arSecondThread.AsyncWaitHandle };
            //WaitHandle[] waitHandles = new WaitHandle[] { arFirstThread.AsyncWaitHandle };

            WaitHandle.WaitAll(waitHandles);

            Console.WriteLine("Operation complete. Press any key to close window...");
            Console.ReadLine();
        }

        private void StartFirstThread()
        {
            var apiInfo = new TwitterApiInfo
            {
                AccessToken = "313315355-DT9HzoZKyLC4WbfKblVh5KAMyE0IGvtx5XFDtUx5",
                AccessTokenSecret = "7BhEPtC9RaQK0VIHmrugaimXXoOYjxxVhZGpcxw1w",
                ConsumerKey = "hYd5c8gYxhy3T2Nvj8wLA",
                ConsumerSecret = "KZGPvAN6kgioWlf0zrGYOzKevLatexuuuJlIhJQg"
            };

            var provider = new TwitterDataProvider(apiInfo);
            try
            {
                var result = provider.GetUrlOfUserImage("kamorin_roman", TwitterDataProvider.ImageSize.Small);
                var messages = provider.GetUserHomeTimeLine(10);
            }
            catch (Exception) { }

            Console.WriteLine("First thread complete");
        }

        static private void StartSecondThread()
        {
            var errorsCount = 0;
            var apiInfo = new TwitterApiInfo
            {
                AccessToken = "333874355-YBK9kp8Qc0mtRuYp9org5XegCX3DZKwycJdZZd8l",
                AccessTokenSecret = "q0qjlrXlehPag6mNqamvMhoLtb6z2bPUPDRh27HKN7Y",
                ConsumerKey = "hYd5c8gYxhy3T2Nvj8wLA",
                ConsumerSecret = "KZGPvAN6kgioWlf0zrGYOzKevLatexuuuJlIhJQg"
            };

            var provider = new TwitterDataProvider(apiInfo);
            for (var i = 0; i < MaxIterations; i++)
            {
                Thread.Sleep(100);
                try
                {
                    provider.GetUserHomeTimeLine(1);
                    Console.WriteLine("Second thread: Success. Iteration {0} ", i + 1);
                }
                catch (Exception)
                {
                    Console.WriteLine("Second thread: Exception. Iteration {0} ", i + 1);
                    errorsCount++;

                    if (errorsCount == MaxErrorsCount)
                    {
                        Console.WriteLine("Second thread: Exception Max count reached. Breaking. Iteration {0} ", i + 1);
                        break;
                    }
                }
            }

            Console.WriteLine("Second thread complete");
        }
    }
}
#endif