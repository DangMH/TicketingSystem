Challenge: Create and Test a Ticketing System  
==
We need the backend of an online event ticketing system.  The system should be robust enough to allow thousands of customers to access thousands of tickets.  Create a Visual Studio solution with two C# projects.  One project is the backend system.  The other project will test the backend system.
 
For this challenge, no need to have things online or stored in an actual database.  It’s really about the logic, design and coding.  This could certainly grow to be a very elaborate system but today’s exercise is a simple proof of concept.
 
Backend
--

Customers should be able to freely place tickets in their cart but tickets are not locked until customer is ready to check out.  Tickets are unavailable after a successful purchase.  While such a system could grow to be very complex with dozens of tables, ours should be very simple with perhaps only a ticket and a member table.  For example:
 
-          Ticket: id, section, row, seat#

-          Member: id, Name

 
Ultimately, the design is completely up to you.  Your database could be simple lists maintained in memory or something more elaborate like a MySQL database.  Mainly, the system should be capable of dealing with simultaneous access from thousands of customers.  This definitely will require synchronizing access.  I recommend C#’s asych and await keywords.
 
Please make this a C# DLL project whose interface is a class that allows the following functionality:
-          Create a new set of tickets

-          Allow tickets to be locked

-          Allow tickets to be purchased (again, simple; no credit card or currency) or unlocked

 
Testing
--

The second project needs to simulate real-life use.  It will obtain an interface from the backend DLL and use it hammer the backend.
 
This project should be a C# application.  No UI is necessary.