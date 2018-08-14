namespace dto
{
    public class IsEligibleResult
    {
        public CheckEligibleResult CheckEligibleResult { get; set; }
    }

    public class CheckEligibleResult
    {
        public string Reason { get; set; }
        public int ResultType { get; set; }
        public int StatusCode { get; set; }
    }
}
