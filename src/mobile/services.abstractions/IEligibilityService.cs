using System;
using System.Collections.Generic;
using System.Text;
using DTO;

namespace services.abstractions
{
    public interface IEligibilityService
    {
        bool IsDateCanBeCheckedForEligibility(DateTime date);
    }
}
