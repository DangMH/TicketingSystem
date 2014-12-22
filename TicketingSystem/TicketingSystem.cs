using System.Threading.Tasks;

namespace TicketingSystem
{
    /// <summary>
    /// Front-facing API for the dll
    /// </summary>
    public class TicketingSystem
    {
        private const string defaultRepositoryFilePath = @"C:\Users\DangMH\documents\visual studio 2015\Projects\TicketingSystem\TicketingSystem\TicketDB.sdf";
        private TicketRepository ticketRepository = null;

        public TicketingSystem() : this(defaultRepositoryFilePath)
        {

        }

        public TicketingSystem(string repositoryFilePath)
        {
            ticketRepository = new TicketRepository(repositoryFilePath);
        }

        /// <summary>
        /// Initializes the repository
        /// </summary>
        public void Init()
        {
            ticketRepository.InitTables();
        }

        /// <summary>
        /// Creates a ticket with the provided parameters.  Returns the true if ticket is created successfully, else false.
        /// </summary>
        public async Task<bool> CreateTicketAsync(int section, int row, int seat)
        {
            return await ticketRepository.CreateTicketAsync(section, row, seat) >= TicketRepository.MINIMUM_ID;
        }

        /// <summary>
        /// Creates a member with the provided name.  Returns the true if member is created successfully, else false.
        /// </summary>
        public async Task<bool> CreateMemberAsync(string memberName)
        {
            return await ticketRepository.CreateMemberAsync(memberName) >= TicketRepository.MINIMUM_ID;
        }

        /// <summary>
        /// Holds a ticket for purchase.  Returns true if the ticket is free, else false.
        /// </summary>
        public async Task<bool> HoldTicket(int section, int row, int seat, string memberName)
        {
            return await ticketRepository.HoldTicketAsync(section, row, seat, memberName);
        }

        /// <summary>
        /// Purcheses and releases a held ticket.  Returns true if the member is holding the ticket for purchase.
        /// </summary>
        public async Task<bool> PurchaseTicket(int section, int row, int seat, string memberName)
        {
            return await ticketRepository.PurchaseTicketAsync(section, row, seat, memberName);
        }

        /// <summary>
        /// Releases a ticket. Returns true if ticket is currently held by the member, else false.
        /// </summary>
        public async Task<bool> FreeTicket(int section, int row, int seat, string memberName)
        {
            return await ticketRepository.FreeTicketAsync(section, row, seat, memberName);
        }
    }
}
