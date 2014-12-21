using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicketingSystem.E2ETest
{
    public class TestTicket
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static HashSet<int> sections = new HashSet<int>();
        private static HashSet<int> rows = new HashSet<int>();
        private static HashSet<int> seats = new HashSet<int>();

        /// <summary>
        /// Generates a random ticket
        /// </summary>
        public TestTicket()
        {
            while (sections.Contains(section = random.Next())) ;
            sections.Add(section);

            while (rows.Contains(row = random.Next()));
            rows.Add(row);

            while (seats.Contains(seat = random.Next()));
            seats.Add(seat);
        }

        public int section { get; set; }

        public int row { get; set; }

        public int seat { get; set; }
    }
}