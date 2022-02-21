﻿using System;

namespace BulkyBook.Utility
{
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
    }
}
