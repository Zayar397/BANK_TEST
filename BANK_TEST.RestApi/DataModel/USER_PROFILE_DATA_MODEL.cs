namespace BANK_TEST.RestApi.DataModel
{
    public class USER_PROFILE_DATA_MODEL
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string MobileNo { get; set; }

        public decimal Balance { get; set; }

        public string Pin { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }
    }
    public class DEPOSTI_WITHDRAW_REQ
    {
        public string MobileNo { get; set; }

        public decimal Balance { get; set; }
    }
    public class TRANSFER_REQ
    {
        public string frMobileNo { get; set; }

        public string toMobileNo { get; set; }

        public decimal Balance { get; set; }

        public string Pin { get; set; }
    }
}
