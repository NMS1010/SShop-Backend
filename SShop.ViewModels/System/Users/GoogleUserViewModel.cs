namespace SShop.ViewModels.System.Users
{
    public class GoogleUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Given_name { get; set; }
        public string Family_name { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}