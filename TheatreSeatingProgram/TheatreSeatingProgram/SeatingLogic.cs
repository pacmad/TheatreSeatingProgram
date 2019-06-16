using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheatreSeatingProgram.Models;
namespace TheatreSeatingProgram
{
    interface ISeatingLogic
    {
        List<SeatLayout> ReadInputSeatLayouts(string filepath, out int emptyLineCounter);
        List<CustomerRequest> ReadCustomerRequests(string filepath, int emptyLineCounter);
        void parseLayoutRequests(List<SeatLayout> seatLayouts, List<CustomerRequest> customerRequests);
    }
    class SeatingLogic : ISeatingLogic
    {
        public List<SeatLayout> ReadInputSeatLayouts(string filepath, out int emptyLineCounter)
        {
            List<SeatLayout> _seatlayouts = new List<SeatLayout>();

            string[] allLines = File.ReadAllLines(filepath);
            int i;
            int rowCounter = 0;
            for (i = 0; i <= allLines.Length; i++)
            {
                if (allLines[i].Trim() == "")
                    break;
                string[] splitSections = allLines[i].Split(' ');
                for (int j = 0; j < splitSections.Length; j++)
                    _seatlayouts.Add(new SeatLayout
                    {
                        RowNumber = rowCounter,
                        ColumnNumber = j,
                        SectionSeatCount = Convert.ToInt32(splitSections[j])
                    });
                rowCounter++;
            }
            emptyLineCounter = i;

            return _seatlayouts;
        }
        public List<CustomerRequest> ReadCustomerRequests(string filepath, int emptyLineCounter)
        {
            List<CustomerRequest> _custrequests;
            string[] allLines = File.ReadAllLines(filepath);
            _custrequests = new List<CustomerRequest>();
            for (int j = emptyLineCounter + 1; j < allLines.Length; j++)
            {
                string[] splitCustomerRequest = allLines[j].Split(' ');

                _custrequests.Add(new CustomerRequest
                {
                    Name = splitCustomerRequest[0],
                    Seats = Convert.ToInt32(splitCustomerRequest[1])
                });
            }
            return _custrequests;
        }
        public void parseLayoutRequests(List<SeatLayout> seatLayouts, List<CustomerRequest> customerRequests)
        {
            int totalseats = seatLayouts.Select(a => a.SectionSeatCount).Sum();
            foreach (var eachrequest in customerRequests)
            {
                if (eachrequest.Seats < totalseats)
                {
                    var seatLayoutsSelection = (from row in seatLayouts
                                                where row.SectionSeatCount > 0 &&
                                                      row.SectionSeatCount >= eachrequest.Seats
                                                select row).ToList();

                    if (seatLayoutsSelection.Count > 0)
                    {
                        var StartRow = (from x in seatLayoutsSelection select x.RowNumber).Min();
                        var NextRowCount = (from x in seatLayoutsSelection select x.RowNumber).SkipWhile(x => x <= StartRow).ToList();
                        int NextRow = NextRowCount.Count() > 0 ? NextRowCount.Min() : StartRow;

                        var ExactMatch = (from x in seatLayoutsSelection
                                          where x.RowNumber >= StartRow &&
                                                x.RowNumber <= NextRow &&
                                                x.SectionSeatCount == eachrequest.Seats
                                          select x).ToList();

                        if (NextRow - StartRow <= 1 & ExactMatch.Count != 0)
                        {
                            var ExactMatch1 = ExactMatch.FirstOrDefault();
                            seatLayouts.Where(p => p.RowNumber == ExactMatch1.RowNumber && p.ColumnNumber == ExactMatch1.ColumnNumber).Select(u => { u.SectionSeatCount -= eachrequest.Seats; return u; }).ToList();
                            Console.WriteLine(eachrequest.Name + " Row " + (ExactMatch1.RowNumber + 1) + " Section " + (ExactMatch1.ColumnNumber + 1));
                        }
                        else
                        {
                            var SelectColSection = (from x in seatLayoutsSelection select x.ColumnNumber).ToList().FirstOrDefault();
                            seatLayouts.Where(p => p.RowNumber == StartRow && p.ColumnNumber == SelectColSection).Select(u => { u.SectionSeatCount -= eachrequest.Seats; return u; }).ToList();
                            Console.WriteLine(eachrequest.Name + " Row " + (StartRow + 1) + " Section " + (SelectColSection + 1));
                        }
                    }
                    else
                    {
                        Console.WriteLine(eachrequest.Name + " Call to split party.");
                    }
                }
                else
                    Console.WriteLine(eachrequest.Name + " Sorry, we can't handle your party.");

            }
        }
    }
}
