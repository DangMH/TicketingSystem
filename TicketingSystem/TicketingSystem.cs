using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem
{
    public class TicketingSystem
    {
        private TicketRepository ticketRepository;

        /// <summary>
        /// Creates a ticket with the provided parameters.  Returns the ID of the created ticket.
        /// </summary>
        public int CreateTicket(int section, int row, int seat)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Holds a ticket for purchase.  Returns true if the ticket is free, else false.
        /// </summary>
        public async Task<bool> HoldTicket(int ticketID, int memberID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Purcheses and releases a held ticket.  Returns true if the member is holding the ticket for purchase.
        /// </summary>
        public async Task<bool> PurchaseTicket(string ticketID, string memberID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Releases a ticket. Returns true if ticket is currently held by the member, else false.
        /// </summary>
        public async Task<bool> FreeTicket()
        {
            throw new System.NotImplementedException();
        }
    }
}
