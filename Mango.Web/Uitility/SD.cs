namespace Mango.Web.Uitility
{
    public class SD
    {
        public static string CouponApIBase { get; set; }
        public static string AuthApIBase { get; set; }

        public const string RoleAdmin = "ADMIN";

        public const string RoleCustomer = "CUSTOMER";

        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }
    }
}
