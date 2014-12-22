using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TicketingSystem.E2ETest
{
    public class TestThread
    {
        private enum Activity
        {
            CREATE_MEMBER,
            CREATE_TICKET,
            HOLD_TICKET,
            PURCHASE_TICKET,
            FREE_TICKET,
            HOLD_FREE_TICKET,
            HOLD_PURCHASE_TICKET
        }

        private static Activity[] activitySet = {
            Activity.CREATE_MEMBER,
            Activity.CREATE_TICKET,
            Activity.FREE_TICKET,
            Activity.HOLD_TICKET,
            Activity.PURCHASE_TICKET,
            Activity.HOLD_FREE_TICKET,
            Activity.HOLD_PURCHASE_TICKET};

        private static ConcurrentQueue<TestMember> availableMemberList = null;
        private static ConcurrentQueue<TestTicket> availableTicketList = null;
        private static ConcurrentBag<TestMember> createdMembers = null;
        private static ConcurrentBag<TestTicket> createdTickets = null;
        private static TicketingSystem ticketingSystem = null;
        private static int purchasedTickets = -1;
        private static int ticketsToPurchase = -1;
        private static bool initialized = false;

        private int ID = -1;
        private CountDownLatch latch = null;
        private Random random = null;

        static TestThread()
        {
            createdMembers = new ConcurrentBag<TestMember>();
            createdTickets = new ConcurrentBag<TestTicket>();

            ticketingSystem = new TicketingSystem();
            ticketingSystem.Init();

            purchasedTickets = 0;
            initialized = false;
        }

        public TestThread(int ID, CountDownLatch latch)
        {
            this.ID = ID;
            this.latch = latch;
            random = new Random((int)DateTime.Now.Ticks + ID);

            Console.WriteLine("\tThread " + ID + " created");
        }

        /// <summary>
        /// Clears the repository database entries.
        /// </summary>
        public static void Init(ConcurrentQueue<TestMember> memberList, ConcurrentQueue<TestTicket> ticketList)
        {
            if (initialized)
            {
                return;
            }

            availableMemberList = memberList;
            availableTicketList = ticketList;
            ticketsToPurchase = availableTicketList.Count;

            initialized = true;
        }

        /// <summary>
        /// Selects a random activity to run and completes the action regardless of projected failure
        /// </summary>
        public async void ThreadProc()
        {
            bool activitySuccess = false;

            while (initialized && purchasedTickets < ticketsToPurchase)
            {
                TestMember member = null;
                TestTicket ticket = null;
                activitySuccess = PeekItems(out member, out ticket);

                Console.Write("\tThread " + ID + ": ");

                switch (SelectActivity())
                {
                    case Activity.CREATE_MEMBER:
                        Console.WriteLine("Creating Member");
                        activitySuccess = await CreateMember();
                        break;
                    case Activity.CREATE_TICKET:
                        Console.WriteLine("Creating Ticket");
                        activitySuccess = await CreateTicket();
                        break;
                    case Activity.FREE_TICKET:
                        Console.WriteLine("Freeing Ticket");
                        activitySuccess = activitySuccess ? await FreeTicket(member, ticket) : false;
                        break;
                    case Activity.HOLD_TICKET:
                        Console.WriteLine("Holding Ticket");
                        activitySuccess = activitySuccess ? await HoldTicket(member, ticket) : false;
                        break;
                    case Activity.PURCHASE_TICKET:
                        Console.WriteLine("Purchasing Ticket");
                        activitySuccess = activitySuccess ? await PurchaseTicket(member, ticket) : false;
                        break;
                    case Activity.HOLD_FREE_TICKET:
                        Console.WriteLine("Holding and Freeing Ticket");
                        activitySuccess = activitySuccess ? await HoldTicket(member, ticket) : false;
                        activitySuccess = activitySuccess ? await FreeTicket(member, ticket) : false;
                        break;
                    case Activity.HOLD_PURCHASE_TICKET:
                        Console.WriteLine("Holding and Purchasing Ticket");
                        activitySuccess = activitySuccess ? await HoldTicket(member, ticket) : false;
                        activitySuccess = activitySuccess ? await PurchaseTicket(member, ticket) : false;
                        break;
                    default:
                        break;
                }

                Console.WriteLine("Activity Successful: " + activitySuccess);
            }

            Console.WriteLine("\tThread " + ID + " Finished Signal");
            latch.Signal();
            return;
        }

        /// <summary>
        /// Creates a random member from the available member pool.
        /// </summary>
        private async Task<bool> CreateMember()
        {
            TestMember member = null;

            if (!availableMemberList.TryDequeue(out member))
            {
                // Unable to retrieve a member
                return false;
            }

            if (!(await ticketingSystem.CreateMemberAsync(member.name)))
            {
                // Unable to create member.  Restore to availableMember list
                availableMemberList.Enqueue(member);
                return false;
            }

            // Add member to random access container
            createdMembers.Add(member);

            return true;
        }

        /// <summary>
        /// Creates a random ticket from the available ticketpool.
        /// </summary>
        private async Task<bool> CreateTicket()
        {
            TestTicket ticket = null;

            if (!availableTicketList.TryDequeue(out ticket))
            {
                // Unable to retrieve a ticket
                return false;
            }

            if (!(await ticketingSystem.CreateTicketAsync(ticket.section, ticket.row, ticket.seat)))
            {
                // Unable to create ticket.  Restore to availableTicket list
                availableTicketList.Enqueue(ticket);
                return false;
            }

            // Add ticket to random access container
            createdTickets.Add(ticket);

            return true;
        }

        /// <summary>
        /// Frees a random ticket held by a random member from the created pools.
        /// </summary>
        private async Task<bool> FreeTicket(TestMember member, TestTicket ticket)
        {
            if (!(await ticketingSystem.FreeTicket(ticket.section, ticket.row, ticket.seat, member.name)))
            {
                // unable to free the designated ticket
                return false;
            }

            return true;
        }

        /// <summary>
        /// Holds a random ticket from the created tickets pool.
        /// </summary>
        private async Task<bool> HoldTicket(TestMember member, TestTicket ticket)
        {
            if (!(await ticketingSystem.HoldTicket(ticket.section, ticket.row, ticket.seat, member.name)))
            {
                // unable to free the designated ticket
                return false;
            }

            return true;
        }

        /// <summary>
        /// Purchases a random ticket held by a random member from the created pools
        /// </summary>
        private async Task<bool> PurchaseTicket(TestMember member, TestTicket ticket)
        {
            if (!(await ticketingSystem.PurchaseTicket(ticket.section, ticket.row, ticket.seat, member.name)))
            {
                // unable to free the designated ticket
                return false;
            }

            ++purchasedTickets;
            Console.WriteLine("Thread " + ID + ": ticket purchased, " + purchasedTickets + " out of " + ticketsToPurchase);

            return true;
        }

        /// <summary>
        /// Grabs and reorders items from the set of created tickets and members
        /// </summary>
        private bool PeekItems(out TestMember testMember, out TestTicket testTicket)
        {
            testMember = null;
            testTicket = null;

            if (!createdMembers.TryTake(out testMember))
            {
                // unable to get a member
                Console.WriteLine("\t\tThread " + ID + ": No Member Found");
                return false;
            }
            // Return to bag for reordering
            createdMembers.Add(testMember);

            if (!createdTickets.TryTake(out testTicket))
            {
                // unable to get a ticket, return member to bag
                Console.WriteLine("\t\tThread " + ID + ": No Ticket Found");
                return false;
            }
            // Return to bag for reordering
            createdTickets.Add(testTicket);

            Console.WriteLine("\t\tThread " + ID + ": Member(" + testMember.name + "), Ticket(" + testTicket.section + "," + testTicket.row + "," + testTicket.seat + ")");

            return true;
        }

        /// <summary>
        /// Selects a random activity for the thread to emulate
        /// </summary>
        private Activity SelectActivity()
        {
            return activitySet[random.Next(0, activitySet.Length)];
        }
    }
}
