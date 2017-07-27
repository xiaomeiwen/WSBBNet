using System;
using WSBB;
using UnityEngine;


namespace XMLSpace
{
	
	public class Team
	{
	    public String teamName;
	    public DefaultPlayer[] players;
	    public Team()
	    {
			players = new DefaultPlayer[3];
	    }
	}
}