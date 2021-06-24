//-----------------------------------------------------------------------
// <copyright file="DoHomeErrorCode.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Error codes from a DoHome device.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed.")]
    public enum DoHomeErrorCode
    {
        ERR_WIFI_NONE = 0,
        ERR_WIFI_SCAN_FAILED = 1,
        ERR_WIFI_SCAN_TIMEOUT = 2,
        ERR_WIFI_INVALID_PASSWORD = 3,
        ERR_WIFI_GET_CMD_FAILED = 4,
        ERR_WIFI_GET_STATUS_FAILED = 5,
        ERR_WIFI_SCAN_RES_NULL = 6,
        ERR_WIFI_GET_PASSWORD_FAILED = 7,
        ERR_WIFI_GET_LED_OP_FAILED = 8,
        ERR_WIFI_GET_SSID_FAILED = 9,
        ERR_WIFI_GET_RED_FAILED = 10,
        ERR_WIFI_GET_BLUE_FAILED = 11,
        ERR_WIFI_GET_GREEN_FAILED = 12,
        ERR_WIFI_GET_WHITE_FAILED = 13,
        ERR_WIFI_GET_M_FAILED = 14,
        ERR_WIFI_GET_MODE_INDEX_FAILED = 15,
        ERR_WIFI_GET_FREQ_FAILED = 16,
        ERR_WIFI_GET_TIME_JSON_FAILD = 17,
        ERR_WIFI_GET_YEAR_FAILED = 18,
        ERR_WIFI_GET_MONTH_FAILED = 19,
        ERR_WIFI_GET_DAY_FAILED = 20,
        ERR_WIFI_GET_HOUR_FAILED = 21,
        ERR_WIFI_GET_MINUTE_FAILED = 22,
        ERR_WIFI_GET_SECOND_FAILED = 23,
        ERR_WIFI_MALLOC_FAILED = 24,
        ERR_WIFI_SET_SHUTDOWN_TIMER_FAILED = 25,
        ERR_WIFI_UNKNOWN_CMD = 26,
        ERR_WIFI_GET_TIMER_INDEX_FAILED = 27,
        ERR_WIFI_GET_DELAY_TIME_FAILED = 28,
        ERR_WIFI_GET_TYPE_FAILED = 29,
        ERR_WIFI_SET_TIMER_FAILED = 30,
        ERR_WIFI_GET_COLOR_ARR_FAILED = 31,
        ERR_WIFI_TOO_MANY_CUSTOM_COLOR = 32,
        ERR_WIFI_GET_OP_FAILED = 33,
        ERR_WIFI_CHANGE_RMT_CTRL_FAILED = 34,
        ERR_WIFI_GET_POWERUP_INFO_FAILED = 35,
        ERR_WIFI_MODIFY_TIMER_FAILED = 36,
        ERR_WIFI_GET_MODE_LIST_FAILED = 37,
        ERR_WIFI_GET_MODE_ITEM_FAILED = 38,
        ERR_WIFI_GET_LOOP_FAILED = 39,
        ERR_WIFI_TOO_MANY_MODE = 40,
        ERR_WIFI_GET_REPEAT_FAILED = 41,
        ERR_WIFI_GET_TIMER_INFO_FAILED = 42,
        ERR_WIFI_GET_TIMEZONE_OFF_FAILED = 43,
        ERR_WIFI_NOT_CONN_ROUTER = 44,
        ERR_WIFI_GET_TIMESTAMP_FAILED = 45,
        ERR_WIFI_GET_REPEATER_EN_FAILED = 46,
        ERR_WIFI_GET_PORT_FAILED = 47,
        ERR_WIFI_GET_VAL_FAILED = 48,
        ERR_WIFI_GET_IS_ON_FAILED = 49,
        ERR_WIFI_GET_WEEKDAY_FAILED = 50,
        ERR_WIFI_GET_WRONG_WEEKDAY = 51,
        ERR_WIFI_GET_WEEKDAY_ITEM_FAILED = 52,
        ERR_WIFI_GET_ERROR_TIMER_TYPE = 53,
    }
}
