namespace BakongHealthCheck.Dto.MBService
{
    public class ResponseV1DTOResponseMBServiceDTO
    {
        // Response status code and message 
        public string userID { get; set; }
        public string serviceID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string blackListVersion { get; set; }
    }
}
