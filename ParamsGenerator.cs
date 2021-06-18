using System.Collections.Generic;

namespace Report_Update_Automation
{
    public static class ParamsGenerator
    {
        public static string[] reportParameters =
        {
            "PARAM_RoomAssignment",
            "PARAM_MembershipType",
            "PARAM_MembershipLevel",
            "PARAM_Preferences",
            "PARAM_Marketcode",
            "PARAM_Channel",
            "PARAM_Source",
            "PARAM_RatePlan",
            "PARAM_RateValueFrom",
            "PARAM_RateValueTo",
            "PARAM_VIP",
            "PARAM_ReservationStatus",
            "PARAM_GuaranteeType",
            "PARAM_ReservationIndicators",
            "PARAM_ShowIncognito",
            "Incl_Notes",
            "PARAM_NoteTypes",
            "Incl_PaymentMethod",
            "Incl_RateAmount",
            "Incl_AssignedAndDNM",
            "Incl_shares",
            "Incl_AccompanyingGuests",
            "Incl_StayStatistics",
            "Incl_Transportation",
            "Incl_OpenTasks",
            "Incl_AllTasks",
            "Incl_CustomFields",
            "PARAM_CustomFields",
            "Incl_ExternalID",
            "Incl_Balance",
            "Incl_BillingInstructions",
            "Incl_Preferences",
            "Alternate",
            "SortBy",
            "GroupBy",
        };

        public static List<string> GenerateReportParameteres(string[] reportParametersToGenerate)
        {
            var toReturn = new List<string>();
            
            for (int i = 0; i < reportParametersToGenerate.Length; i++)
            {
                toReturn.Add(@$"new ReportDefinitionParameterPredefinedValue({15 + i}, ""SYS_RES_022"", ""{reportParametersToGenerate[i]}"" , ""0"", 3),");
            }

            return toReturn;
        }
    }
}