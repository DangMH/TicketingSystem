using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicketingSystem.E2ETest
{
    public class TestMember
    {
        private const int MAX_NAME_LENGTH = 50;
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static HashSet<string> names = new HashSet<string>();

        /// <summary>
        /// Generates a random member
        /// </summary>
        public TestMember()
        {
            while (names.Contains(name = RandomString(random.Next(1, MAX_NAME_LENGTH))));
            names.Add(name);
        }

        public string name { get; set; }

        /// <summary>
        /// Random string generator referenced from http://stackoverflow.com/questions/9995839/how-to-make-random-string-of-numbers-and-letters-with-a-length-of-5
        /// </summary>
        private string RandomString(int Size)
        {

            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}