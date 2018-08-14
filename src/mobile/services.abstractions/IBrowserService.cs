using System;
using System.Threading.Tasks;
using DTO;

namespace services.abstractions
{
    public interface IBrowserService
    {
        void OpenServiceGuaranteePage(TripFromTo trip, DateTime dateTime, string from, string to, string prestoCardNumber);
    }
}
