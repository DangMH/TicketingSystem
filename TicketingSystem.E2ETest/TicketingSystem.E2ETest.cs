using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace TicketingSystem.E2ETest
{
    public class E2ETest
    {
        /// <summary>
        /// Runs the program
        /// </summary>
        public static void Main(string[] args)
        {
            RunE2ETest();

            Console.WriteLine("Enter Any Key to Exit");
            Console.Read();
        }

        /// <summary>
        /// Queries the user for inputs on threads, number of members and number of tickets to test against
        /// </summary>
        public static void RunE2ETest()
        {
            ConcurrentQueue<TestMember> memberList = null;
            ConcurrentQueue<TestTicket> ticketList = null;

            Console.WriteLine("Enter the number of test threads: ");
            int numThreads = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the number of test members: ");
            int numMembers = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the number of test tickets: ");
            int numTickets = Convert.ToInt32(Console.ReadLine());

            memberList = new ConcurrentQueue<TestMember>();
            ticketList = new ConcurrentQueue<TestTicket>();

            Console.WriteLine("Member List:");
            for (int i = 0; i < numMembers; ++i)
            {
                TestMember tempMember = new TestMember();
                memberList.Enqueue(tempMember);

                Console.WriteLine("\t- " + tempMember.name);
            }
            Console.WriteLine();

            Console.WriteLine("Ticket List:");
            for (int i = 0; i < numTickets; ++i)
            {
                TestTicket tempTicket = new TestTicket();
                ticketList.Enqueue(tempTicket);

                Console.WriteLine("\t- " + tempTicket.section + " " + tempTicket.row + " " + tempTicket.seat);
            }
            Console.WriteLine();

            TestThread.Init(memberList, ticketList);

            CountDownLatch latch = new CountDownLatch(numThreads);
            for (int i = 0; i < numThreads; ++i)
            {
                TestThread testThread = new TestThread(i, latch);

                Thread t = new Thread(new ThreadStart(testThread.ThreadProc));

                Console.WriteLine("Main: created thread " + i);
                t.Start();
            }

            Console.WriteLine("Waiting on all threads to return...");
            latch.Wait();
        }
    }
}
