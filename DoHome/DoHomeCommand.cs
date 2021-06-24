namespace DoHome
{
	/// <summary>
	/// All the different API commands.
	/// </summary>
    internal enum DoHomeCommand
    {
		INVALID_CMD = 0,
		GET_WIFI_SCAN_RES = 1,              
		MODIFY_SSID = 2,                    
		REBOOT = 3,                         
		GET_DEV_INFO = 4,                   
		LED_OPERATE = 5,                    
		CHANGE_COLOR = 6,                   
		SET_PRESET_MODE = 7,                
		SET_CUSTOM_MODE = 8,                
		GET_DEV_TIME = 9,                   
		SYNC_DEV_TIME = 10,                 
		SET_POWERUP_LED_STATUS = 11,        
		REMEMBER_SHUTDOWN_LED_STATUS = 12,  
		SET_SHUTDOWN_TIMER = 13,            
		SET_POWERUP_TIMER = 14,             
		REMOTE_CONTROL_ENABLE = 15,         
		ROUTER_CONFIG = 16,                 
		DELAY_SHUTDOWN = 17,                
		START_OTA = 18,                     
		IS_CONNECT_TO_ROUTER = 19,          
		GET_VERSION = 20,                   
		GET_DEV_TIMER = 21,                 
		GET_DELAY_INFO = 22,                
		CANCEL_TIMER = 23,                  
		GET_POWERUP_STATUS = 24,            
		GET_LED_STATUS = 25,                
		MODIFY_TIMER = 26,                  
		PRESET_MODE_COMBO = 27,             
		RESET_AP = 28,                      
		SET_TIMEZONE = 29,                  
		ENABLE_REPEATER = 30,               
		ENABLE_PORTAL = 31,                 
		SET_PORTAL_TEXT = 32,               
		SET_LIGHT_PERCENT = 33,             
		SET_TO_PERCENT = 34,                
		SET_IR_GPIO = 35,                   
	}
}
