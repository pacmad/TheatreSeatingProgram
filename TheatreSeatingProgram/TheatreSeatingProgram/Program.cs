using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheatreSeatingProgram.Models;

namespace TheatreSeatingProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            string filepath = "C:\\Input\\TheatreLayoutInput.txt";
            ISeatingLogic obj = new SeatingLogic();
            List<SeatLayout> _seatLayouts = obj.ReadInputSeatLayouts(filepath, out int emptyLineCounter);
            List<CustomerRequest> _customerRequests = obj.ReadCustomerRequests(filepath, emptyLineCounter);
            obj.parseLayoutRequests(_seatLayouts, _customerRequests);
            Console.Read();
        }
    }
}
