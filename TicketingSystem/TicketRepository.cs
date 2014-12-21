using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Threading.Tasks;

namespace TicketingSystem
{
    public enum TicketStatus
    {
        FREE,
        HELD,
        PURCHASED
    }

    public class TicketRepository
    {
        public string connectionFilePath = null;

        private TicketRepository()
        {

        }

        public TicketRepository(string filePath)
        {
            connectionFilePath = filePath;
        }

        /// <summary>
        /// Creates a ticket
        /// </summary>
        public int CreateTicket(int section, int row, int seat)
        {
            int newTicketID = 0; // Default settings for PK are (1,1).  ID's < 1 are invalid

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                connection.Open();

                // TODO: Establish unique item

                string query = "INSERT INTO Tickets(SECTION, ROW, SEAT) VALUES('"
                    + section + "','"
                    + row + "','"
                    + seat + "')";

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;

                if (command.ExecuteNonQuery() <= 0)
                {
                    // Potential to use ID's < 1 as ERROR Codes
                    newTicketID = -1;
                }
                else
                {
                    // Get the last created record's PK scoped to the current connection for thread safety
                    query = "SELECT @@SCOPE_IDENTITY";
                    command = new SqlCeCommand();
                    command.Connection = connection;
                    command.CommandText = query;

                    newTicketID = Convert.ToInt32((decimal)command.ExecuteScalar());
                }
            }

            return newTicketID;
        }

        /// <summary>
        /// Adds a ticket/member mapping to the appropriate table
        /// </summary>
        private async Task<int> AddTicketMapping(int ticketID, int memberID, TicketStatus ticketStatus)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes a ticket/member mapping from the appropriate table
        /// </summary>
        private async Task<bool> DeleteTicketMapping(int ticketID, string memberID, TicketStatus ticketStatus)
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
        /// Purchases a ticket.  Returns true if the ticket is held by the memberm, else false.
        /// </summary>
        public async Task<bool> PurchaseTicket(int ticketID, int memberID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Frees a ticket.  Returns true if the ticket is held by the member, else false.
        /// </summary>
        public async Task <bool> FreeTicket(int ticketID, int memberID)
        {
            throw new System.NotImplementedException();
        }
    }
}