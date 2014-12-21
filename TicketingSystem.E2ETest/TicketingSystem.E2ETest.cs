using System;
using System.Collections.Generic;

namespace TicketingSystem.E2ETest
{
    public class E2ETest
    {
        public static void Main(string[] args)
        {
            List<TestMember> memberList = null;
            List<TestTicket> ticketList = null;

            Console.WriteLine("Enter the number of concurrent test members: ");
            int numMembers = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the number of test tickets: ");
            int numTickets = Convert.ToInt32(Console.ReadLine());

            memberList = new List<TestMember>(numMembers);
            ticketList = new List<TestTicket>(numTickets);

            Console.WriteLine("Member List:");
            for (int i = 0; i < numMembers; ++i)
            {
                TestMember tempMember = new TestMember();
                memberList.Add(tempMember);

                Console.WriteLine("\t- " + tempMember.name);
            }
            Console.WriteLine();

            Console.WriteLine("Ticket List:");
            for (int i = 0; i < numMembers; ++i)
            {
                TestTicket tempTicket = new TestTicket();
                ticketList.Add(tempTicket);

                Console.WriteLine("\t- " + tempTicket.section + " " + tempTicket.row + " " + tempTicket.seat);
            }
            Console.WriteLine();

            RunTest(memberList, ticketList);

            Console.WriteLine("Enter Any Key to Exit");
            Console.Read();
        }

        public async static void RunTest(List<TestMember> memberList, List<TestTicket> ticketList)
        {
        }
    }
}
