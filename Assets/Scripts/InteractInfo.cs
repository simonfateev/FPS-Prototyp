using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InteractInfo
{
    public PlayerScript byPlayer;
	public PlayerScript.Side side;

	public InteractInfo(PlayerScript playerScript, PlayerScript.Side side)
	{
		this.byPlayer = playerScript;
		this.side = side;
	}
}
