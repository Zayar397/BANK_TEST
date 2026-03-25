namespace BANK_TEST.RestApi.ViewModel
{
    public class USER_PROFILE_VIEW_MODEL
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string MobileNo { get; set; }

        public decimal Balance { get; set; }

        public string Pin { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
