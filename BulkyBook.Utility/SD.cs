﻿namespace BulkyBook.Utility;

public static class SD
{
    public const string SP_CoverTypes_SelectAll = "dbo.usp_CoverTypes_SelectAll";
    public const string SP_CoverTypes_Select = "dbo.usp_CoverTypes_Select";
    public const string SP_CoverTypes_Insert = "dbo.usp_CoverTypes_Insert";
    public const string SP_CoverTypes_Update = "dbo.usp_CoverTypes_Update";
    public const string SP_CoverTypes_Delete = "dbo.usp_CoverTypes_Delete";

    public const string IdentityRole_Customer_Individual = "Individual Customer";
    public const string IdentityRole_Customer_Company = "Company Customer";
    public const string IdentityRole_Admin = "Admin";
    public const string IdentityRole_Employee = "Employee";

    public const string Session_ShoppingCart = "Shopping Cart Session";

    public const string OrderStatus_Pending = "Pending";
    public const string OrderStatus_Approved = "Approved";
    public const string OrderStatus_InProcess = "Processing";
    public const string OrderStatus_Shipped = "Shipped";
    public const string OrderStatus_Cancelled = "Cancelled";
    public const string OrderStatus_Refunded = "Refunded";

    public const string PaymentStatus_Pending = "Pending";
    public const string PaymentStatus_Approved = "Approved";
    public const string PaymentStatus_DelayedPayment = "ApprovedForDelayedPayment";
    public const string PaymentStatus_Rejected = "Rejected";
    public const string PaymentStatus_Canceled = "Canceled";
    public const string PaymentStatus_Refunded = "Refunded";

    public static decimal GetPriceBasedOnQuantity(int quantity, decimal price, decimal price50, decimal price100) => quantity switch
    {
        (>= 0) and (< 50) => price,
        (>= 50) and (< 100) => price50,
        (>= 100) => price100,
        _ => 0.0m
    };

    public static string ConvertToRawHtml(string source)
    {
        char[] array = new char[source.Length];
        int arrayIndex = 0;
        bool inside = false;
        for (int i = 0; i < source.Length; i++)
        {
            char let = source[i];
            if (let == '<')
            {
                inside = true;
                continue;
            }
            if (let == '>')
            {
                inside = false;
                continue;
            }
            if (!inside)
            {
                array[arrayIndex++] = let;
            }
        }
        return new string(array, 0, arrayIndex);
    }
}