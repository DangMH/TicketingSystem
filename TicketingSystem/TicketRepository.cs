using System;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Threading.Tasks;

namespace TicketingSystem
{
    /// <summary>
    /// Class representing the database file and containsn the interface to interact with the appropriate elements.
    /// </summary>
    public class TicketRepository
    {
        private enum TicketStatus
        {
            FREE,
            HELD,
            PURCHASED
        }

        public const int MINIMUM_ID = 1;
        private string connectionFilePath = null;

        private TicketRepository()
        {

        }

        public TicketRepository(string filePath)
        {
            connectionFilePath = @"DataSource=" + filePath;
        }

        /// <summary>
        /// Drops all entries from each table
        /// </summary>
        public void InitTables()
        {
            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                connection.Open();

                string[] tables = { "Tickets", "Members", "TicketsFree", "TicketsHeld", "TicketsPurchased" };

                string query = "DELETE FROM ";

                foreach (string tableName in tables)
                {
                    SqlCeCommand command = new SqlCeCommand();
                    command.Connection = connection;
                    command.CommandText = query + tableName;

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Creates a ticket
        /// </summary>
        public async Task<int> CreateTicketAsync(int section, int row, int seat)
        {
            int newTicketID = MINIMUM_ID - 1; // Default settings for PK are (1,1).  ID's < 1 are invalid

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                await connection.OpenAsync();

                // TODO: Establish unique item

                string query = "INSERT INTO Tickets(T_SECTION,T_ROW,T_SEAT) VALUES("
                    + section + ", "
                    + row + ", "
                    + seat + ")";

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;

                if ((await command.ExecuteNonQueryAsync()) < MINIMUM_ID)
                {
                    // Potential to use ID's < 1 as ERROR Codes
                    newTicketID = -1;
                }
                else
                {
                    // Get the last created record's PK scoped to the current connection for thread safety
                    query = "SELECT @@IDENTITY";
                    command = new SqlCeCommand();
                    command.Connection = connection;
                    command.CommandText = query;

                    newTicketID = Convert.ToInt32((decimal)command.ExecuteScalar());
                }

                connection.Close();
            }

            if ((await AddTicketMappingAsync(newTicketID, MINIMUM_ID, TicketStatus.FREE)) < MINIMUM_ID)
            {
                // Something Illegal happened
                return newTicketID;
            }

            return newTicketID;
        }

        /// <summary>
        /// Creates a member
        /// </summary>
        public async Task<int> CreateMemberAsync(string memberName)
        {
            int newMemberID = MINIMUM_ID - 1; // Default settings for PK are (1,1).  ID's < 1 are invalid

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                await connection.OpenAsync();

                // TODO: Establish unique item

                string query = "INSERT INTO Members(NAME) VALUES('" + memberName + "')";

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;

                if ((await command.ExecuteNonQueryAsync()) < MINIMUM_ID)
                {
                    // Potential to use ID's < 1 as ERROR Codes
                    newMemberID = -1;
                }
                else
                {
                    // Get the last created record's PK scoped to the current connection for thread safety
                    query = "SELECT @@IDENTITY";
                    command = new SqlCeCommand();
                    command.Connection = connection;
                    command.CommandText = query;

                    newMemberID = Convert.ToInt32((decimal)command.ExecuteScalar());
                }

                connection.Close();
            }

            return newMemberID;
        }

        /// <summary>
        /// Adds a ticket/member mapping to the appropriate table
        /// </summary>
        private async Task<int> AddTicketMappingAsync(int ticketID, int memberID, TicketStatus ticketStatus)
        {
            int newTicketMappingID = 0;

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                await connection.OpenAsync();

                // TODO: Establish unique item

                string query = BuildInsertTicketTableQuery(ticketID, memberID, ticketStatus);

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;

                if ((await command.ExecuteNonQueryAsync()) < MINIMUM_ID)
                {
                    // Potential to use ID's < 1 as ERROR Codes
                    newTicketMappingID = -1;
                }
                else
                {
                    // Get the last created record's PK scoped to the current connection for thread safety
                    query = "SELECT @@IDENTITY";
                    command = new SqlCeCommand();
                    command.Connection = connection;
                    command.CommandText = query;

                    newTicketMappingID = Convert.ToInt32((decimal)command.ExecuteScalar());
                }

                connection.Close();
            }

            return newTicketMappingID;
        }

        /// <summary>
        /// Deletes a ticket/member mapping from the appropriate table
        /// </summary>
        private async Task<bool> DeleteTicketMappingAsync(int ticketID, int memberID, TicketStatus ticketStatus)
        {
            bool success = false;

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                await connection.OpenAsync();

                string query = BuildDeleteTicketTableQuery(ticketID, memberID, ticketStatus);

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;
                if ((await command.ExecuteNonQueryAsync()) >= MINIMUM_ID)
                {
                    success = true;
                }

                connection.Close();
            }

            return success;
        }

        /// <summary>
        /// Holds a ticket for purchase.  Returns true if the ticket is free, else false.
        /// </summary>
        public async Task<bool> HoldTicketAsync(int section, int row, int seat, string memberName)
        {
            return await TransferTicketMappingAsync(section, row, seat, memberName, TicketStatus.FREE, TicketStatus.HELD);
        }

        /// <summary>
        /// Purchases a ticket.  Returns true if the ticket is held by the memberm, else false.
        /// </summary>
        public async Task<bool> PurchaseTicketAsync(int section, int row, int seat, string memberName)
        {
            return await TransferTicketMappingAsync(section, row, seat, memberName, TicketStatus.HELD, TicketStatus.PURCHASED);
        }

        /// <summary>
        /// Frees a ticket.  Returns true if the ticket is held by the member, else false.
        /// </summary>
        public async Task<bool> FreeTicketAsync(int section, int row, int seat, string memberName)
        {
            return await TransferTicketMappingAsync(section, row, seat, memberName, TicketStatus.HELD, TicketStatus.FREE);
        }

        /// <summary>
        /// Gets the appropriate table name according to the ticket status.
        /// </summary>
        private string GetTicketTableName(TicketStatus ticketStatus)
        {
            string tableName = null;

            switch (ticketStatus)
            {
                case TicketStatus.FREE:
                    tableName = "TicketsFree";
                    break;
                case TicketStatus.HELD:
                    tableName = "TicketsHeld";
                    break;
                case TicketStatus.PURCHASED:
                    tableName = "TicketsPurchased";
                    break;
                default:
                    break;
            }

            return tableName;
        }

        /// <summary>
        /// Build the insert query for the ticket mapping table
        /// </summary>
        private string BuildInsertTicketTableQuery(int ticketID, int memberID, TicketStatus ticketStatus)
        {
            string tableName = GetTicketTableName(ticketStatus);

            string query = "INSERT INTO " + tableName + "(";

            switch (ticketStatus)
            {
                case TicketStatus.FREE:
                    query += "TICKET_ID";
                    break;
                case TicketStatus.HELD:
                    query += "TICKET_ID,MEMBER_ID";
                    break;
                case TicketStatus.PURCHASED:
                    query += "TICKET_ID,MEMBER_ID";
                    break;
                default:
                    break;
            }

            query += ") VALUES(";

            switch (ticketStatus)
            {
                case TicketStatus.FREE:
                    query += ticketID;
                    break;
                case TicketStatus.HELD:
                    query += ticketID + "," + memberID;
                    break;
                case TicketStatus.PURCHASED:
                    query += ticketID + "," + memberID;
                    break;
                default:
                    break;
            }

            query += ")";

            return query;
        }

        /// <summary>
        /// Build the delete query for the ticket mapping table
        /// </summary>
        private string BuildDeleteTicketTableQuery(int ticketID, int memberID, TicketStatus ticketStatus)
        {
            string tableName = GetTicketTableName(ticketStatus);

            string query = "DELETE FROM " + tableName + " WHERE ";

            switch (ticketStatus)
            {
                case TicketStatus.FREE:
                    query += "TICKET_ID=" + ticketID;
                    break;
                case TicketStatus.HELD:
                    query += "TICKET_ID=" + ticketID + " AND MEMBER_ID=" + memberID;
                    break;
                case TicketStatus.PURCHASED:
                    query += "TICKET_ID=" + ticketID + " AND MEMBER_ID=" + memberID;
                    break;
                default:
                    break;
            }

            return query;
        }

        /// <summary>
        /// Gets the ticket id of the associated seat parameters
        /// </summary>
        private async Task<int> GetTicketID(int section, int row, int seat)
        {
            int ticketID = 0;

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                await connection.OpenAsync();

                // TODO: Establish unique item

                string query = "SELECT ID FROM Tickets WHERE T_SECTION=" + section
                    + " AND T_ROW=" + row
                    + " AND T_SEAT=" + seat;

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;

                DbDataReader reader = await command.ExecuteReaderAsync();

                try
                {
                    while (reader.Read())
                    {
                        ticketID = reader.GetInt32(0);
                    }
                }
                finally
                {
                    reader.Close();
                }

                connection.Close();
            }

            return ticketID;
        }

        /// <summary>
        /// Gets the member id of the associated name
        /// </summary>
        private async Task<int> GetMemberID(string memberName)
        {
            int memberID = 0;

            using (SqlCeConnection connection = new SqlCeConnection(connectionFilePath))
            {
                await connection.OpenAsync();

                // TODO: Establish unique item

                string query = "SELECT ID FROM Members WHERE NAME='" + memberName + "'";

                SqlCeCommand command = new SqlCeCommand();
                command.Connection = connection;
                command.CommandText = query;

                DbDataReader reader = await command.ExecuteReaderAsync();

                try
                {
                    while (reader.Read())
                    {
                        memberID = reader.GetInt32(0);
                    }
                }
                finally
                {
                    reader.Close();
                }

                connection.Close();
            }

            return memberID;
        }

        /// <summary>
        /// Transfers a ticket mapping from one table to another.
        /// </summary>
        private async Task<bool> TransferTicketMappingAsync(int section, int row, int seat, string memberName, TicketStatus sourceTable, TicketStatus destTable)
        {
            int ticketID = await GetTicketID(section, row, seat);
            if (MINIMUM_ID > ticketID)
            {
                // Ticket does not exist.
                return false;
            }

            int memberID = await GetMemberID(memberName);
            if (MINIMUM_ID > memberID)
            {
                // Member does not exist.
                return false;
            }

            // Try validate source table mapping as well as remove entry
            if (!(await DeleteTicketMappingAsync(ticketID, memberID, sourceTable)))
            {
                // Mapping does not exist
                return false;
            }

            // Add destination table mapping
            if (MINIMUM_ID > (await AddTicketMappingAsync(ticketID, memberID, destTable)))
            {
                // Add should not have failed.  Something illegal happened.
                // Possible logic for cleanup here
                return false;
            }

            return true;
        }
    }
}