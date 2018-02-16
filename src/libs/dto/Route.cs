using DTO.Abstractions;
using System;

namespace DTO
{
    public class Route : IEntity
    {
        public string RouteId { get; set; }
        public string AgencyId { get; set; }
        public string RouteShortName { get; set; }
        public string RouteLongName { get; set; }
        public int RouteType { get; set; }
        public string RouteColor { get; set; }
        public string RouteTextColor { get; set; }
    }
}
