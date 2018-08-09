using UnityEngine;
using System.Collections;

public class UserInfoManager : MonoBehaviour {
	//Client _gameclient = null;
	
	string _username = "";
	string _userid = "";
	bool _loggedin = false;
	
	
	//singleton value
	UserInfoManager _self = null;
	public UserInfoManager UIM {
		get{ return _self; }
	}
	
	// singleton and persistance setup
	void Awake()
	{
		if( _self != null)
		{
			Destroy( this );
		}
		else
		{
			_self = this;
			DontDestroyOnLoad( this );
		}
	}
	
	//used for logging in for an existing user
	public string GetUserID()
	{
		return _userid;
	}
	public string GetUsername()
	{
		return _username;
	}
	public bool LogIn( string username, string password )
	{	
		return LogIn( username, password);
	}
	public bool LogIn( string username, string password, bool saveinfo )
	{
		
		/*PlayerIO.QuickConnect.SimpleConnect(
			"ascension-sze3hsku0ifmjkzvirc2w",
			username,
			password,
			loginCallback
		);
		*/
		return false;
	}
	
	/*public void loginCallback(Client c)
	{
		_gameclient = c;
	}
	*/
	//used for creating an account
	public bool UsernameExists( string username )
	{
		return false;
	}
	public bool IsEmail( string email )
	{
		return false;
	}
	public bool IsValidPassword( string password)
	{
		return false;
	}
	public bool Register( string username, string password, string email )
	{
		return false;
	}
	
	//used to log out
	public bool LogOut()
	{
		if(!_loggedin) return false;
		
		_username = "";
		_userid = "";
		_loggedin = false;
		
		return true;
	}
}
