using System;
using System.Collections.Concurrent;

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
            FREE_TICKET
        }

        private static Activity[] activitySet = {
            Activity.CREATE_MEMBER,
            Activity.CREATE_TICKET,
            Activity.FREE_TICKET,
            Activity.HOLD_TICKET,
            Activity.PURCHASE_TICKET };

        private static ConcurrentQueue<TestMember> availableMemberList = null;
        private static ConcurrentQueue<TestTicket> availableTicketList = null;
        private static ConcurrentDictionary<int, TestMember> createdMembers = null;
        private static ConcurrentDictionary<int, TestTicket> createdTickets = null;
        private static TicketingSystem ticketingSystem = null;
        private static int purchasedTickets = -1;
        private static int ticketsToPurchase = -1;
        private static bool initialized = false;

        private int ID = -1;
        private CountDownLatch latch = null;
        private Random random = null;

        static TestThread()
        {
            createdMembers = new ConcurrentDictionary<int, TestMember>();
            createdTickets = new ConcurrentDictionary<int, TestTicket>();
            ticketingSystem = new TicketingSystem();
            purchasedTickets = 0;
            initialized = false;
        }

        public TestThread(int ID, CountDownLatch latch)
        {
            this.ID = ID;
            this.latch = latch;
            random = new Random((int)DateTime.Now.Ticks + ID);

            Console.WriteLine("Thread " + ID + " created");
        }

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

        public async void ThreadProc()
        {
            while (initialized && purchasedTickets < ticketsToPurchase)
            {
                switch(SelectActivity())
                {
                    case Activity.CREATE_MEMBER:
                        break;
                    case Activity.CREATE_TICKET:
                        break;
                    case Activity.FREE_TICKET:
                        break;
                    case Activity.HOLD_TICKET:
                        break;
                    case Activity.PURCHASE_TICKET:
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("Thread " + ID + " Signaling");
            latch.Signal();
            return;
        }

        private Activity SelectActivity()
        {
            return activitySet[random.Next(0, activitySet.Length)];
        }
    }
}
