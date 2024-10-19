using System;

namespace Team_12.DTOs
{
    public class EventDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int FacilityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal EventPrice { get; set; }
        public int Capacity { get; set; }
    }
}