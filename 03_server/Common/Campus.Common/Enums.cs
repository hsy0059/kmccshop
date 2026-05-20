namespace Campus.Common;

public static class UserType
{
    public const int Student = 1;
    public const int Merchant = 2;
    public const int Rider = 3;
    public const int Admin = 4;
}

public static class OrderStatus
{
    public const int PendingPayment = 1;
    public const int PendingAccept = 2;
    public const int Accepted = 3;
    public const int Delivering = 4;
    public const int Delivered = 5;
    public const int Cancelled = 6;
    public const int Completed = 7;
}

public static class ErrandOrderStatus
{
    public const int Pending = 1;
    public const int Accepted = 2;
    public const int Delivering = 3;
    public const int Completed = 4;
    public const int Cancelled = 5;
}

public static class RiderStatus
{
    public const int Offline = 0;
    public const int Online = 1;
    public const int Delivering = 2;
    public const int Disabled = 3;
}

public static class RiderAuditStatus
{
    public const int Pending = 0;
    public const int Approved = 1;
    public const int Rejected = 2;
}

public static class WithdrawStatus
{
    public const int Pending = 1;
    public const int Approved = 2;
    public const int Processing = 3;
    public const int Completed = 4;
    public const int Rejected = 5;
}

public static class CouponType
{
    public const int FullReduction = 1;
    public const int Discount = 2;
    public const int NoThreshold = 3;
}

public static class UserCouponStatus
{
    public const int Unused = 1;
    public const int Used = 2;
    public const int Expired = 3;
}

public static class WalletLogType
{
    public const int Recharge = 1;
    public const int Expense = 2;
    public const int Refund = 3;
    public const int Income = 4;
    public const int Withdraw = 5;
    public const int Freeze = 6;
    public const int Unfreeze = 7;
}

public static class FeedbackType
{
    public const int Problem = 1;
    public const int Suggestion = 2;
    public const int Other = 3;
}

public static class FeedbackStatus
{
    public const int Pending = 1;
    public const int Processing = 2;
    public const int Replied = 3;
    public const int Closed = 4;
}

public static class LostFoundType
{
    public const int Lost = 1;
    public const int Found = 2;
}

public static class LostFoundStatus
{
    public const int Normal = 1;
    public const int Returned = 2;
    public const int Expired = 3;
}

public static class SecondGoodsStatus
{
    public const int OnSale = 1;
    public const int Sold = 2;
}

public static class MerchantStatus
{
    public const int Pending = 0;
    public const int Open = 1;
    public const int Rest = 2;
    public const int Disabled = 3;
}

public static class AdvertisementPosition
{
    public const string Banner = "banner";
    public const string Home = "home";
    public const string Sidebar = "sidebar";
}
